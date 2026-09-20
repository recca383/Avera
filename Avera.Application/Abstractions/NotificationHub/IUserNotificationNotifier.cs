using Avera.Application.Users.Notifications;
using System;

namespace Avera.Application.Abstractions.NotificationHub
{
    public interface IUserNotificationNotifier
    {
        Task NotifyUserSuspendedAsync(UserSuspendedNotification notification, CancellationToken cancellationToken = default);

        Task NotifyUserRemovedAsync(UserRemovedNotification notification, CancellationToken cancellationToken = default);

        Task NotifyUserDeletedAsync(UserDeletedNotification notification, CancellationToken cancellationToken = default);
    }
}
