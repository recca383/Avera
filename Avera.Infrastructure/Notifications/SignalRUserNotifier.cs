using Avera.Application.Abstractions.NotificationHub;
using Avera.Application.Users.Notifications;
using Avera.Application.Hubs;
using Microsoft.AspNetCore.SignalR;
namespace Avera.Infrastructure.Notifications
{
    internal sealed class SignalRUserNotifier(
        IHubContext<NotificationHub> hubContext
    ) : IUserNotificationNotifier
    {
        public async Task NotifyUserSuspendedAsync(UserSuspendedNotification notification, CancellationToken cancellationToken = default)
        {
            await hubContext
                .Clients
                .Group($"user:{notification.UserId}")
                .SendAsync("UserSuspended", notification, cancellationToken);
        }

        public async Task NotifyUserRemovedAsync(UserRemovedNotification notification, CancellationToken cancellationToken = default)
        {
            await hubContext
                .Clients
                .Group($"user:{notification.UserId}")
                .SendAsync("UserRemoved", notification, cancellationToken);
        }

        public async Task NotifyUserDeletedAsync(UserDeletedNotification notification, CancellationToken cancellationToken = default)
        {
            // Notify tenant admins that a user deleted their account
            if (notification.TenantId.HasValue)
            {
                await hubContext
                    .Clients
                    .Group($"tenant:{notification.TenantId}:admins")
                    .SendAsync("UserDeleted", notification, cancellationToken);
            }
        }

        public async Task NotifyUserUnsuspendedAsync(UserUnsuspendedNotification notification, CancellationToken cancellationToken = default)
        {
            await hubContext
                .Clients
                .Group($"user:{notification.UserId}")
                .SendAsync("UserUnsuspended", notification, cancellationToken);
        }
    }
}
