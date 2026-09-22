using Avera.Application.Abstractions.Queues;
using Avera.Application.Cases.Create;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Authentication;
using SharedKernel;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Avera.Domain.Application.Cases;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Avera.Infrastructure.Queues;

internal sealed class CaseCreationQueue : ICaseCreationQueue, IDisposable
{
    private readonly Channel<QueueItem> _channel = Channel.CreateUnbounded<QueueItem>(new UnboundedChannelOptions { SingleReader = true, AllowSynchronousContinuations = false });
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _worker;
    private readonly IServiceProvider _serviceProvider;

    public CaseCreationQueue(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _worker = Task.Run(ProcessQueueAsync);
    }

    public async Task<Result<Domain.Application.Cases.Case>> EnqueueAsync(CreateCaseCommand command, Guid callerUserId, Guid? callerTenantId, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<Result<Domain.Application.Cases.Case>>(TaskCreationOptions.RunContinuationsAsynchronously);

        var item = new QueueItem(command, callerUserId, callerTenantId, tcs);

        await _channel.Writer.WriteAsync(item, cancellationToken);

        using var linked = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, cancellationToken);
        try
        {
            return await tcs.Task.WaitAsync(linked.Token);
        }
        catch (OperationCanceledException)
        {
            return Result.Failure<Domain.Application.Cases.Case>(new Error("Queue.Canceled", "Case creation canceled", ErrorType.Failure));
        }
    }

    private async Task ProcessQueueAsync()
    {
        await foreach (var item in _channel.Reader.ReadAllAsync(_cts.Token))
        {
            try
            {
                using var scope = ((IServiceProvider)_serviceProvider).GetService(typeof(Microsoft.Extensions.DependencyInjection.IServiceScopeFactory)) is Microsoft.Extensions.DependencyInjection.IServiceScopeFactory factory ? factory.CreateScope() : throw new InvalidOperationException("IServiceScopeFactory not available");
                var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<Domain.Identity.Users.User>>();
                var dateTime = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

                // Recreate handler-like logic synchronously to ensure ordering
                if (item.CallerTenantId == null)
                {
                    item.Tcs.SetResult(Result.Failure<Domain.Application.Cases.Case>(Avera.Domain.Identity.Tenants.TenantErrors.NotMember));
                    continue;
                }

                var user = await userManager.FindByIdAsync(item.CallerUserId.ToString());

                if (user!.IsSuspended)
                {
                    item.Tcs.SetResult(Result.Failure<Domain.Application.Cases.Case>(Avera.Domain.Identity.Users.UserErrors.IsSuspended));
                    continue;
                }

                // Enforce per-user daily creation limit if set
                if (user.DailyCaseLimit > 0)
                {
                    var todayCount = await db.Cases
                        .IgnoreQueryFilters()
                        .CountAsync(
                            c => c.TenantId == item.CallerTenantId &&
                                 c.CreatedByUserId == item.CallerUserId &&
                                 c.CreatedAt.Date == dateTime.PhilippineNow.Date,
                            CancellationToken.None);

                    if (todayCount >= user.DailyCaseLimit)
                    {
                        item.Tcs.SetResult(Result.Failure<Domain.Application.Cases.Case>(new SharedKernel.Error("User.DailyLimitReached", "Daily case creation limit reached", SharedKernel.ErrorType.Conflict)));
                        continue;
                    }
                }

                // Generate case code deterministically under lock (single reader provides sequencing)
                Domain.Application.Cases.Case? newCase = null;

                for (int attempt = 0; attempt < 5; attempt++)
                {
                    newCase = new Domain.Application.Cases.Case()
                    {
                        Id = Guid.NewGuid(),
                        CaseCode = await GenerateCaseCodeAsync(db, dateTime),
                        SubjectName = item.Command.SubjectName,
                        CreatedByUserId = item.CallerUserId,
                        TenantId = item.CallerTenantId,
                        DocumentType = item.Command.DocumentType,
                        Priority = item.Command.Priority,
                        Notes = "",
                        CreatedAt = dateTime.PhilippineNow,
                        Status = Domain.Application.Cases.Status.Processing
                    };

                    try
                    {
                        await db.Cases.AddAsync(newCase, CancellationToken.None);
                        await db.SaveChangesAsync(CancellationToken.None);

                        item.Tcs.SetResult(Result.Success(newCase));
                        newCase = null;
                        break;
                    }
                    catch (DbUpdateException ex) when (
                        ex.InnerException is PostgresException postgresException &&
                        postgresException.SqlState == PostgresErrorCodes.UniqueViolation)
                    {
                        //db.Entry(newCase).State = EntityState.Detached;

                        if (attempt == 4)
                        {
                            item.Tcs.SetResult(
                                Result.Failure<Domain.Application.Cases.Case>(
                                    new Error(
                                        "Case.CodeGenerationFailed",
                                        "Unable to generate a unique case code.",
                                        ErrorType.Failure)));

                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                item.Tcs.SetResult(Result.Failure<Domain.Application.Cases.Case>(new Error("Queue.Failure", ex.Message, ErrorType.Failure)));
            }
        }
    }

    private static async Task<string> GenerateCaseCodeAsync(
        IApplicationDbContext db,
        IDateTimeProvider dateTime)
    {
        var today = dateTime.PhilippineNow.Date;

        var numberOfCasesToday = await db.Cases
            .IgnoreQueryFilters()
            .CountAsync(
                c => c.CreatedAt.Date == today,
                CancellationToken.None);

        return $"CASE-{dateTime.PhilippineNow:MMddyyyy}-{numberOfCasesToday + 1:D3}";
    }

    public void Dispose()
    {
        _cts.Cancel();
        _channel.Writer.TryComplete();
        try { _worker.Wait(5000); } catch { }
    }

    private sealed class QueueItem
    {
        public CreateCaseCommand Command { get; }
        public Guid CallerUserId { get; }
        public Guid? CallerTenantId { get; }
        public TaskCompletionSource<Result<Domain.Application.Cases.Case>> Tcs { get; }

        public QueueItem(CreateCaseCommand command, Guid callerUserId, Guid? callerTenantId, TaskCompletionSource<Result<Domain.Application.Cases.Case>> tcs)
        {
            Command = command;
            CallerUserId = callerUserId;
            CallerTenantId = callerTenantId;
            Tcs = tcs;
        }
    }
}
