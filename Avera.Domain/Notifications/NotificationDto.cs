using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Domain.Notifications
{
    public sealed record NotificationDto(
        Guid Id,
        string Type,
        string Title,
        string Message,
        string? ResourceId,
        bool IsRead,
        DateTimeOffset CreatedAt);
}
