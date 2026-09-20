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
    internal sealed class MemberRequestApprovedDomainEventHandler(
        IMemberRequestNotifier memberRequestNotifier,
        UserManager<User> userManager,
        IIdentityDbContext identityDbContext,
        IEmailService emailService
    ) : IDomainEventHandler<MemberRequestApprovedDomainEvent>
    {
        public async Task Handle(MemberRequestApprovedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(domainEvent.UserId.ToString());
            var admin = await userManager.FindByIdAsync(domainEvent.ReviewedByUserId.ToString());
            var tenant = await identityDbContext.Tenants.FindAsync(new object[] { domainEvent.TenantId }, cancellationToken);

            var notification = new MemberRequestApprovedNotification(
                domainEvent.MemberRequestId,
                domainEvent.UserId,
                user?.FirstName ?? string.Empty,
                user?.LastName ?? string.Empty,
                user?.Email ?? string.Empty,
                domainEvent.TenantId,
                domainEvent.ReviewedByUserId,
                domainEvent.ReviewedAt
            );

            await memberRequestNotifier.NotifyMemberRequestApprovedAsync(notification, cancellationToken);

            // preserve previous behavior: send approval email
            if (user != null && tenant != null && admin != null)
            {
                await emailService.SendRequestApprovedAsync(
                    user.Email!,
                    user.FirstName!,
                    tenant.Name,
                    admin.FirstName + " " + admin.LastName,
                    cancellationToken);
            }
        }
    }
}
