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
    : ICaseNotificationNotifier
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

        public async Task NotifyCaseFlaggedAsync(
            Guid caseOwnerUserId,
            CaseFlaggedNotification notification,
            CancellationToken cancellationToken)
        {
            try
            {
                await hubContext
                    .Clients
                    .Group($"user:{caseOwnerUserId}")
                    .SendAsync("CaseFlagged", notification, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to send CaseFlagged notification for Case {CaseId}",
                    notification.CaseId);
            }
        }

        public async Task NotifyCaseResultCreatedAsync(
            Guid tenantId,
            NewCaseResultNotification notification,
            CancellationToken cancellationToken)
        {
            try
            {
                await hubContext
                    .Clients
                    .Group($"tenant:{tenantId}:admins")
                    .SendAsync("NewCaseResult", notification, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to send NewCaseResult notification for Case {CaseId}",
                    notification.CaseId);
            }
        }
    }
}
