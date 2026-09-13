using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using SharedKernel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Avera.Domain.Notifications;

namespace Avera.Application.Notifications.MarkAsRead
{
    internal sealed class MarkNotificationAsReadCommandHandler
        (
            IApplicationDbContext applicationDbContext,
            IUserContext userContext,
            IDateTimeProvider dateTime
        ): ICommandHandler<MarkNotificationAsReadCommand>
    {
        public async Task<Result> Handle(MarkNotificationAsReadCommand command, CancellationToken cancellationToken)
        {
            var notification = await applicationDbContext
                                       .Notifications
                                       .SingleOrDefaultAsync(n => n.UserId == userContext.UserId
                                                        && n.TenantId == userContext.TenantId
                                                        && n.Id == command.NotificationId, cancellationToken);

            if (notification == null)
            {
                return Result.Failure(NotificationErrors.NotificationNotFound);
            }

            notification.ReadAt = dateTime.PhilippineNow;
            notification.IsRead = true;

            await applicationDbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
            
        }
    }
}
