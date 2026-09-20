using Avera.Application.Abstractions.Authentication;
using Avera.Application.Infrastructure;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Application.Cases;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SharedKernel;
using Avera.Application.Abstractions.Queues;

namespace Avera.Application.Cases.Create
{
    public sealed class CreateCaseCommandHandler(
        IApplicationDbContext dbContext,
        IUserContext userContext,
        UserManager<User> userManager,
        IDateTimeProvider dateTime
        ) 
        : ICommandHandler<CreateCaseCommand, Case>
    {
        private static readonly ILogger logger = Log.ForContext<CreateCaseCommandHandler>();
        
        public async Task<Result<Case>> Handle(CreateCaseCommand command, CancellationToken cancellationToken)
        {
            // Early-enforce user's daily limit to avoid enqueueing if already reached.
            var user = await userManager.FindByIdAsync(userContext.UserId.ToString());

            if (user is null)
                return Result.Failure<Case>(UserErrors.UserNotFound);

            if (user.DailyCaseLimit.HasValue)
            {
                var todayCount = await dbContext.Cases.CountAsync(c => c.CreatedByUserId == user.Id && c.CreatedAt.Date == dateTime.PhilippineNow.Date, cancellationToken);
                if (todayCount >= user.DailyCaseLimit.Value)
                {
                    return Result.Failure<Case>(new SharedKernel.Error("User.DailyLimitReached", "Daily case creation limit reached", SharedKernel.ErrorType.Conflict));
                }
            }

            // Enqueue creation to preserve ordering of case codes when multiple requests arrive concurrently
            var queue = (Avera.Application.Abstractions.Queues.ICaseCreationQueue)AppServices.ServiceProvider.GetService(typeof(Avera.Application.Abstractions.Queues.ICaseCreationQueue))!;

            var result = await queue.EnqueueAsync(command, userContext.UserId, userContext.TenantId, cancellationToken);

            return result;
        }
    }
}