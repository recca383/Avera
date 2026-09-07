using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Abstractions.Services;
using Avera.Domain.Identity.MemberRequests;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
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
            IEmailService emailService
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

            var user = await userManager.FindByIdAsync(request.UserId.ToString());

            if (user == null) 
            {
                return Result.Failure(UserErrors.UserNotFound);
            }

            user.TenantId = request.TenantId;

            await userManager.AddToRoleAsync(user, "User");

            await identityDbContext.SaveChangesAsync(cancellationToken);

            await userManager.UpdateSecurityStampAsync(user);
            
            await emailService.SendRequestApprovedAsync(user.Email!, cancellationToken);

            return Result.Success();
        }
    }
}
