using Avera.Application.Abstractions.NotificationHub;
using Avera.Application.Cases.Notifications;
using Avera.Domain.Application.Notifications;
using Avera.Domain.Cases.Events;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Cases.Review
{
    internal sealed class ReviewCaseCompletedDomainEventHandler(
        ICaseNotificationNotifier caseNotifier
        ) : IDomainEventHandler<CaseReviewCompletedDomainEvent>
    {
        public async Task Handle(CaseReviewCompletedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var message = new CaseReviewCompletedNotification(
            domainEvent.CaseId,
            domainEvent.FinalVerdict,
            domainEvent.ReviewedAt);

            await caseNotifier.NotifyReviewCompletedAsync(
                domainEvent.AnalystUserId,
                message,
                cancellationToken);
        }
    }
}
