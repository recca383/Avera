using System;

namespace Avera.Application.Users.Notifications
{
    public sealed record UserSuspendedNotification(
        Guid UserId,
        Guid? TenantId,
        DateTime SuspendedAt,
        string Message
    );
}
