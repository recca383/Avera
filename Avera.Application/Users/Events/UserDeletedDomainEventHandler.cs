using Avera.Application.Abstractions.NotificationHub;
using Avera.Application.Users.Notifications;
using Avera.Domain.Identity.Users.Events;
using SharedKernel;

namespace Avera.Application.Users.Events
{
    internal sealed class UserDeletedDomainEventHandler(
        IUserNotificationNotifier userNotifier
    ) : IDomainEventHandler<UserDeletedDomainEvent>
    {
        public async Task Handle(UserDeletedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var notification = new UserDeletedNotification(
                domainEvent.UserId,
                domainEvent.TenantId,
                domainEvent.DeletedAt,
                "A user has deleted their account."
            );

            await userNotifier.NotifyUserDeletedAsync(notification, cancellationToken);
        }
    }
}
