using System;

namespace Avera.Application.Users.Notifications
{
    public sealed record UserRemovedNotification(
        Guid UserId,
        Guid? TenantId,
        Guid RemovedByUserId,
        DateTime RemovedAt,
        string Message
    );
}
