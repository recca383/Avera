using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Notifications.Get
{
    internal sealed class GetNotificationsQueryHandler(
        IApplicationDbContext applicationDbContext,
        IUserContext userContext
        ) : IQueryHandler<GetNotificationsQuery, List<NotificationDto>>
    {
        public async Task<Result<List<NotificationDto>>> Handle(GetNotificationsQuery query, CancellationToken cancellationToken)
        {
            var notifications = await applicationDbContext
                                        .Notifications
                                        .Where(n => n.UserId == userContext.UserId)
                                        .OrderByDescending(n => n.CreatedAt)
                                        .Select(n => new NotificationDto(
                                            n.Id,
                                            n.Type,
                                            n.Title,
                                            n.Message,
                                            n.ResourceId,
                                            n.IsRead,
                                            n.CreatedAt
                                            ))
                                        .ToListAsync(cancellationToken);

            return notifications;
        }
    }
}
