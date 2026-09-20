using Avera.Application.Abstractions.NotificationHub;
using Avera.Application.MemberRequests.Notifications;
using Avera.Domain.Identity.MemberRequests.Events;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using SharedKernel;

namespace Avera.Application.MemberRequests.Events
{
    internal sealed class MemberRequestCreatedDomainEventHandler(
        IMemberRequestNotifier memberRequestNotifier,
        UserManager<User> userManager
    ) : IDomainEventHandler<MemberRequestCreatedDomainEvent>
    {
        public async Task Handle(MemberRequestCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(domainEvent.UserId.ToString());

            var notification = new MemberRequestCreatedNotification(
                domainEvent.MemberRequestId,
                domainEvent.UserId,
                user?.FirstName ?? string.Empty,
                user?.LastName ?? string.Empty,
                user?.Email ?? string.Empty,
                domainEvent.TenantId,
                domainEvent.CreatedAt
            );

            await memberRequestNotifier.NotifyMemberRequestCreatedAsync(notification, cancellationToken);
        }
    }
}
