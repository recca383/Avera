using Avera.Application.Abstractions.NotificationHub;
using Avera.Application.MemberRequests.Notifications;
using Avera.Domain.Identity.MemberRequests.Events;
using Avera.Domain.Identity.Users;
using Avera.Application.Abstractions.Services;
using Microsoft.AspNetCore.Identity;
using Avera.Application.Abstractions.Databases;
using SharedKernel;

namespace Avera.Application.MemberRequests.Events
{
    internal sealed class MemberRequestRejectedDomainEventHandler(
        IMemberRequestNotifier memberRequestNotifier,
        UserManager<User> userManager,
        IIdentityDbContext identityDbContext,
        IEmailService emailService
    ) : IDomainEventHandler<MemberRequestRejectedDomainEvent>
    {
        public async Task Handle(MemberRequestRejectedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(domainEvent.UserId.ToString());
            var admin = await userManager.FindByIdAsync(domainEvent.ReviewedByUserId.ToString());
            var tenant = await identityDbContext.Tenants.FindAsync(new object[] { domainEvent.TenantId }, cancellationToken);

            var notification = new MemberRequestRejectedNotification(
                domainEvent.MemberRequestId,
                domainEvent.UserId,
                user?.FirstName ?? string.Empty,
                user?.LastName ?? string.Empty,
                user?.Email ?? string.Empty,
                domainEvent.TenantId,
                domainEvent.ReviewedByUserId,
                domainEvent.ReviewedAt
            );

            await memberRequestNotifier.NotifyMemberRequestRejectedAsync(notification, cancellationToken);

            // preserve previous behavior: send rejection email
            if (user != null && tenant != null && admin != null)
            {
                await emailService.SendRequestRejectedAsync(
                    user.Email!,
                    user.FirstName!,
                    tenant.Name,
                    admin.FirstName + " " + admin.LastName,
                    admin.Email!,
                    cancellationToken);
            }
        }
    }
}
