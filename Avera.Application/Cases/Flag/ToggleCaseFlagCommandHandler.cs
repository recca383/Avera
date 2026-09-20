using System;
using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Cases.Notifications;
using Avera.Domain.Cases.Events;
using Avera.Domain.Application.Cases;
using Avera.Domain.Identity.Tenants;
using SharedKernel;

namespace Avera.Application.Cases.Flag
{
    internal sealed class ToggleCaseFlagCommandHandler(
        IUserContext userContext,
        IApplicationDbContext dbContext)
        : ICommandHandler<ToggleCaseFlagCommand>
    {
        public async Task<Result> Handle(ToggleCaseFlagCommand command, CancellationToken cancellationToken)
        {
            if (userContext.TenantId == null)
                return Result.Failure(TenantErrors.NotMember);

            var selectedCase = await dbContext.Cases.FindAsync(new object[] { command.CaseId }, cancellationToken);

            if (selectedCase == null)
                return Result.Failure(CaseErrors.CaseNotFound);

            selectedCase.ToggleFlag(userContext.UserId, command.IsFlagged, DateTime.UtcNow);

            selectedCase.Raise(new CaseFlagToggledDomainEvent(
                selectedCase.Id,
                userContext.TenantId.Value,
                userContext.UserId,
                command.IsFlagged,
                DateTime.UtcNow
            ));

            await dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
