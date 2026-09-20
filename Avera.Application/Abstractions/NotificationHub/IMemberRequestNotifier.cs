using Avera.Application.MemberRequests.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Abstractions.NotificationHub
{
    public interface IMemberRequestNotifier
    {
        Task NotifyMemberRequestCreatedAsync(
            MemberRequestCreatedNotification notification,
            CancellationToken cancellationToken = default);

        Task NotifyMemberRequestApprovedAsync(
            MemberRequestApprovedNotification notification,
            CancellationToken cancellationToken = default);

        Task NotifyMemberRequestRejectedAsync(
            MemberRequestRejectedNotification notification,
            CancellationToken cancellationToken = default);
    }
}
