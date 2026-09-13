using Avera.Application.Abstractions.NotificationHub;
using Avera.Application.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Cases.Notifications
{
    public sealed class SignalRCaseNotifier(
    IHubContext<NotificationHub> hubContext,
    ILogger<SignalRCaseNotifier> logger)
    : ICaseNotifier
    {
        public async Task NotifyReviewCompletedAsync(
            Guid analystUserId,
            CaseReviewCompletedNotification notification,
            CancellationToken cancellationToken)
        {
                try
                {
                    await hubContext
                        .Clients
                        .Group($"user:{analystUserId}")
                        .SendAsync(
                            "CaseReviewCompleted",
                            notification,
                            cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "Failed to send CaseReviewCompleted notification for Case {CaseId}",
                        notification.CaseId);
                }
        }
    }
}
