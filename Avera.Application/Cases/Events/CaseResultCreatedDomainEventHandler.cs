using Avera.Application.Abstractions.NotificationHub;
using Avera.Application.Cases.Notifications;
using Avera.Domain.Cases.Events;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Avera.Application.Abstractions.Databases;
using SharedKernel;

namespace Avera.Application.Cases.Events
{
    internal sealed class CaseResultCreatedDomainEventHandler(
        ICaseNotificationNotifier caseNotifier,
        UserManager<User> userManager,
        IIdentityDbContext identityDbContext
    ) : IDomainEventHandler<CaseResultCreatedDomainEvent>
    {
        public async Task Handle(CaseResultCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(domainEvent.CreatedByUserId.ToString());
            var tenant = await identityDbContext.Tenants.FindAsync(new object[] { domainEvent.TenantId }, cancellationToken);

            var notification = new NewCaseResultNotification(
                domainEvent.CaseId,
                domainEvent.CreatedByUserId,
                user?.FirstName ?? string.Empty,
                user?.LastName ?? string.Empty,
                domainEvent.TenantId,
                domainEvent.CreatedAt
            );

            await caseNotifier.NotifyCaseResultCreatedAsync(domainEvent.TenantId, notification, cancellationToken);
        }
    }
}
