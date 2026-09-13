using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Notifications;
using SharedKernel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Notifications.MarkAllAsRead
{
    internal sealed class MarkAllNotificationsAsReadCommandHandler(
        IApplicationDbContext applicationDbContext,
        IUserContext userContext,
        IDateTimeProvider dateTime) : ICommandHandler<MarkAllNotificationsAsReadCommand>
    {
        public async Task<Result> Handle(MarkAllNotificationsAsReadCommand command, CancellationToken cancellationToken)
        {
            var notifications = await applicationDbContext
                                       .Notifications
                                       .Where(n => n.UserId == userContext.UserId)
                                       .OrderByDescending(n => n.CreatedAt)
                                       .ToListAsync(cancellationToken);

            foreach(var notification in notifications)
            {
                notification.ReadAt = dateTime.PhilippineNow;
                notification.IsRead = true;
            }

            await applicationDbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
