using Avera.Application.Abstractions.NotificationHub;
using Avera.Application.Cases.Notifications;
using Avera.Domain.Cases.Events;
using Avera.Application.Abstractions.Databases;
using SharedKernel;
using Avera.Domain.Application.Cases;

namespace Avera.Application.Cases.Events
{
    internal sealed class CaseFlagToggledDomainEventHandler(
        ICaseNotificationNotifier caseNotifier,
        IApplicationDbContext applicationDbContext)
        : IDomainEventHandler<CaseFlagToggledDomainEvent>
    {
        public async Task Handle(CaseFlagToggledDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            // Notify the owner of the case (the user who created it)
            var @case = await applicationDbContext.Cases.FindAsync(new object[] { domainEvent.CaseId }, cancellationToken);

            if (@case == null)
                return;

            var notification = new CaseFlaggedNotification(
                domainEvent.CaseId,
                domainEvent.TenantId,
                domainEvent.ToggledByUserId,
                domainEvent.IsFlagged,
                domainEvent.ToggledAt
            );

            // send to the case owner
            await caseNotifier.NotifyCaseFlaggedAsync(@case.CreatedByUserId, notification, cancellationToken);
        }
    }
}
