using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Abstractions.Services;
using Avera.Domain.Identity.MemberRequests;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.MemberRequests.Approve
{
    internal sealed class ApproveCommandHandler
        (
            IIdentityDbContext identityDbContext,
            IUserContext userContext,
            UserManager<User> userManager,
            IEmailService emailService,
            IDateTimeProvider dateTime
        ): ICommandHandler<ApproveCommand>
    {
        public async Task<Result> Handle(ApproveCommand command, CancellationToken cancellationToken)
        {
            var request = await identityDbContext.MemberRequests
                .FindAsync(new object[] { command.MemberRequestId }, cancellationToken);

            if (request == null)
            {
                return Result.Failure(MemberRequestErrors.MemberRequestNotFound);
            }

            var reviewedByUserId = userContext.UserId;

            request.Approve(reviewedByUserId);

            request.ReviewedAt = dateTime.PhilippineNow;

            var user = await userManager.FindByIdAsync(request.UserId.ToString());

            if (user == null) 
            {
                return Result.Failure(UserErrors.UserNotFound);
            }

            var tenant = await identityDbContext.Tenants.SingleOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken);

            var admin = await userManager.FindByIdAsync(reviewedByUserId.ToString());

            user.TenantId = request.TenantId;

            await userManager.AddToRoleAsync(user, "User");

            // Raise domain event so notifications and emails are handled by event handlers
            request.Raise(new Avera.Domain.Identity.MemberRequests.Events.MemberRequestApprovedDomainEvent(
                request.Id,
                request.TenantId,
                request.UserId,
                reviewedByUserId,
                request.ReviewedAt ?? dateTime.PhilippineNow
            ));

            await identityDbContext.SaveChangesAsync(cancellationToken);

            await userManager.UpdateSecurityStampAsync(user);

            return Result.Success();
        }
    }
}
