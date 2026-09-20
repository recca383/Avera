using Avera.Application.Cases.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Abstractions.NotificationHub
{
    internal interface ICaseNotificationNotifier
    {
        Task NotifyReviewCompletedAsync(
            Guid analystUserId,
            CaseReviewCompletedNotification notification,
            CancellationToken cancellation);

        Task NotifyCaseResultCreatedAsync(
            Guid tenantId,
            NewCaseResultNotification notification,
            CancellationToken cancellationToken);

        Task NotifyCaseFlaggedAsync(
            Guid caseOwnerUserId,
            CaseFlaggedNotification notification,
            CancellationToken cancellationToken);
    }
}
