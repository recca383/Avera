using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Abstractions.Services;
using Avera.Domain.Identity.MemberRequests;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Avera.Application.MemberRequests.Reject
{
    internal sealed class RejectCommandHandler
        (
            IIdentityDbContext identityDbContext,
            IUserContext userContext,
            UserManager<User> userManager,
            IDateTimeProvider dateTime
        ) : ICommandHandler<RejectCommand>
    {
        public async Task<Result> Handle(RejectCommand command, CancellationToken cancellationToken)
        {
            var request = await identityDbContext.MemberRequests
               .FindAsync(new object[] { command.MemberRequestId }, cancellationToken);

            if (request == null)
            {
                return Result.Failure(MemberRequestErrors.MemberRequestNotFound);
            }

            var reviewedByUserId = userContext.UserId;

            request.Reject(reviewedByUserId);

            request.ReviewedAt = dateTime.PhilippineNow;

            var user = await userManager.FindByIdAsync(request.UserId.ToString());

            if (user == null)
            {
                return Result.Failure(UserErrors.UserNotFound);
            }

            var tenant = await identityDbContext.Tenants.SingleOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken);

            var admin = await userManager.FindByIdAsync(reviewedByUserId.ToString());

            // Raise domain event so notifications and emails are handled by event handlers
            request.Raise(new Avera.Domain.Identity.MemberRequests.Events.MemberRequestRejectedDomainEvent(
                request.Id,
                request.TenantId,
                request.UserId,
                reviewedByUserId,
                request.ReviewedAt ?? dateTime.PhilippineNow
            ));

            identityDbContext.MemberRequests.Remove(request);

            await identityDbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
