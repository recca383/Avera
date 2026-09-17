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
    }
}
