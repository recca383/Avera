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
            // Enqueue creation to preserve ordering of case codes when multiple requests arrive concurrently
            // Resolve ICaseCreationQueue from DI via the handler's constructor services
            var queue = (Avera.Application.Abstractions.Queues.ICaseCreationQueue)AppServices.ServiceProvider.GetService(typeof(Avera.Application.Abstractions.Queues.ICaseCreationQueue))!;

            var result = await queue.EnqueueAsync(command, userContext.UserId, userContext.TenantId, cancellationToken);

            return result;
        }
    }
}