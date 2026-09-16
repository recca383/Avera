using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Identity.MemberRequests;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.MemberRequests.Cancel
{
    internal sealed class CancelMemberRequestCommandHandler(
        IIdentityDbContext identityDbContext,
        IUserContext userContext
        ) : ICommandHandler<CancelMemberRequestCommand>
    {
        public async Task<Result> Handle(CancelMemberRequestCommand command, CancellationToken cancellationToken)
        {
            var pendingMemberRequest = await identityDbContext
                .MemberRequests
                .SingleOrDefaultAsync(m => m.UserId == userContext.UserId 
                                        && m.Status == MemberRequestStatus.Pending, 
                                           cancellationToken);
            
            if (pendingMemberRequest == null)
            {
                return Result.Failure(MemberRequestErrors.MemberRequestNotFound);
            }

            pendingMemberRequest.Status = MemberRequestStatus.Cancelled;

            await identityDbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
