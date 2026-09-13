using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Notifications.Get
{
    public sealed record GetNotificationsQuery : IQuery<List<NotificationDto>>;
}
