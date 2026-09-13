using Avera.Domain.Cases;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Cases.Notifications
{
    public sealed record CaseReviewCompletedNotification(
        Guid CaseId,
        FinalVerdict FinalVerdict,
        DateTime ReviewedAt);
}
