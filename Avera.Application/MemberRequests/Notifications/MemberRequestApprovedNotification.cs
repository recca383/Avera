using System;

namespace Avera.Application.MemberRequests.Notifications
{
    public sealed record MemberRequestApprovedNotification(
        Guid RequestId,
        Guid UserId,
        string FirstName,
        string LastName,
        string Email,
        Guid TenantId,
        Guid ReviewedByUserId,
        DateTime ReviewedAt
    );
}
