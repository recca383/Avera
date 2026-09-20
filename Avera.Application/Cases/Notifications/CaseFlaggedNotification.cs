using System;

namespace Avera.Application.Cases.Notifications
{
    public sealed record CaseFlaggedNotification(
        Guid CaseId,
        Guid TenantId,
        Guid ToggledByUserId,
        bool IsFlagged,
        DateTime ToggledAt
    );
}
