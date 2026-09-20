using Avera.Application.Abstractions.NotificationHub;
using Avera.Application.Users.Notifications;
using Avera.Domain.Identity.Users.Events;
using SharedKernel;

namespace Avera.Application.Users.Events
{
    internal sealed class UserRemovedFromTenantDomainEventHandler(
        IUserNotificationNotifier userNotifier
    ) : IDomainEventHandler<UserRemovedFromTenantDomainEvent>
    {
        public async Task Handle(UserRemovedFromTenantDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var notification = new UserRemovedNotification(
                domainEvent.UserId,
                domainEvent.TenantId,
                domainEvent.RemovedByUserId,
                domainEvent.RemovedAt,
                "You have been removed from the organization. Please reach your administrator."
            );

            await userNotifier.NotifyUserRemovedAsync(notification, cancellationToken);
        }
    }
}
