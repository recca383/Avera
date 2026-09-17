using Avera.Application.Abstractions.NotificationHub;
using Avera.Application.Hubs;
using Avera.Application.MemberRequests.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Avera.Infrastructure.Notifications
{
    internal sealed class SignalRMemberRequestNotifier(
        IHubContext<NotificationHub> hubContext
        ) : IMemberRequestNotifier
    {
        public async Task NotifyMemberRequestCreatedAsync(MemberRequestCreatedNotification notification, CancellationToken cancellationToken = default)
        {
            await hubContext
                .Clients
                .Group($"tenant:{notification.TenantId}:admins")
                .SendAsync("MemberRequestCreated", notification, cancellationToken);
        }
    }
}
