using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.MemberRequests.Notifications
{
    public sealed record MemberRequestCreatedNotification
    (
        Guid RequestId,
        Guid UserId,
        string FirstName,
        string LastName,
        string Email,
        Guid TenantId,
        DateTime CreatedAt
    );
}
