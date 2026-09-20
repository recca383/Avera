using Avera.Application.Abstractions.NotificationHub;
using Avera.Application.Users.Notifications;
using Avera.Domain.Identity.Users.Events;
using SharedKernel;

namespace Avera.Application.Users.Events
{
    internal sealed class UserSuspendedDomainEventHandler(
        IUserNotificationNotifier userNotifier
    ) : IDomainEventHandler<UserSuspendedDomainEvent>
    {
        public async Task Handle(UserSuspendedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var notification = new UserSuspendedNotification(
                domainEvent.UserId,
                domainEvent.TenantId,
                domainEvent.SuspendedAt,
                "Your account has been suspended by the organization administrator."
            );

            await userNotifier.NotifyUserSuspendedAsync(notification, cancellationToken);
        }
    }
}
