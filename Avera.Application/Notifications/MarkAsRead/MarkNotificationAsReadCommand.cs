using Avera.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Notifications.MarkAsRead
{
    public sealed record MarkNotificationAsReadCommand(
        Guid NotificationId
        ) : ICommand;
}
