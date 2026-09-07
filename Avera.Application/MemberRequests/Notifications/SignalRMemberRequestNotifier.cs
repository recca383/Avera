using Avera.Application.Abstractions.NotificationHub;
using Avera.Application.Hubs;
using Avera.Application.MemberRequests.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace Avera.WebApi.Endpoints.Admin.MemberRequests
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
