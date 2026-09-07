using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Abstractions.Services;
using Avera.Domain.Identity.MemberRequests;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
            IEmailService emailService
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

            var user = await userManager.FindByIdAsync(request.UserId.ToString());

            if (user == null)
            {
                return Result.Failure(UserErrors.UserNotFound);
            }

            await identityDbContext.SaveChangesAsync(cancellationToken);

            await emailService.SendRequestRejectedAsync(user.Email!, cancellationToken);

            return Result.Success();
        }
    }
}
