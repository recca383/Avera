using System;

namespace Avera.Application.Users.Notifications
{
    public sealed record UserUnsuspendedNotification(
        Guid UserId,
        Guid? TenantId,
        DateTime UnsuspendedAt,
        string Message
    );
}
