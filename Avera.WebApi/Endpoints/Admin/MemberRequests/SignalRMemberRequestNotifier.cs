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

        public async Task NotifyMemberRequestApprovedAsync(MemberRequestApprovedNotification notification, CancellationToken cancellationToken = default)
        {
            await hubContext
                .Clients
                .Group($"user:{notification.UserId}")
                .SendAsync("MemberRequestApproved", notification, cancellationToken);
        }

        public async Task NotifyMemberRequestRejectedAsync(MemberRequestRejectedNotification notification, CancellationToken cancellationToken = default)
        {
            await hubContext
                .Clients
                .Group($"user:{notification.UserId}")
                .SendAsync("MemberRequestRejected", notification, cancellationToken);
        }
    }
}
