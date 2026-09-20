using System;

namespace Avera.Application.Users.Notifications
{
    public sealed record UserDeletedNotification(
        Guid UserId,
        Guid? TenantId,
        DateTime DeletedAt,
        string Message
    );
}
