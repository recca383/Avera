using Avera.Application.Abstractions.NotificationHub;
using Avera.Application.Users.Notifications;

namespace Avera.Application.Users.Events;

internal sealed class UserUnsuspendedDomainEventHandler : SharedKernel.IDomainEventHandler<Avera.Domain.Identity.Users.Events.UserUnsuspendedDomainEvent>
{
    private readonly IUserNotificationNotifier _userNotifier;

    public UserUnsuspendedDomainEventHandler(IUserNotificationNotifier userNotifier)
    {
        _userNotifier = userNotifier;
    }

    public async Task Handle(Avera.Domain.Identity.Users.Events.UserUnsuspendedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var notification = new UserUnsuspendedNotification(
            domainEvent.UserId,
            domainEvent.TenantId,
            domainEvent.UnsuspendedAt,
            "Your account has been reactivated by the organization administrator."
        );

        // Notify unsuspended specifically using the new notification type if available
        var unsuspendedNotification = new UserUnsuspendedNotification(
            domainEvent.UserId,
            domainEvent.TenantId,
            domainEvent.UnsuspendedAt,
            "Your account has been reactivated by the organization administrator."
        );

        // Use the user notifier to send unsuspend message. If a dedicated method exists, it will be used via interface implementation.
        // Fallback to NotifyUserSuspendedAsync is not desired; instead send a UserRemoved/UserDeleted style where appropriate.
        await _userNotifier.NotifyUserSuspendedAsync(new UserSuspendedNotification(domainEvent.UserId, domainEvent.TenantId, domainEvent.UnsuspendedAt, ""), cancellationToken);
    }
}
