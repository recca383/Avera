using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Cases.Get;
using Avera.Domain.Identity.MemberRequests;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.MemberRequests.GetPending
{
    internal sealed class GetPendingQueryHandler
        (
            IIdentityDbContext identityDbContext,
            IUserContext userContext
        ): IQueryHandler<GetPendingQuery, List<GetPendingQueryResponse>>
    {
        public async Task<Result<List<GetPendingQueryResponse>>> Handle(GetPendingQuery query, CancellationToken cancellationToken)
        {
            if (userContext.TenantId == null)
                return Result.Failure<List<GetPendingQueryResponse>>(TenantErrors.NotMember);

            var memberRequests = await identityDbContext.MemberRequests
                .Where(mr => mr.Status == MemberRequestStatus.Pending)
                .Include(mr => mr.User)
                .Select(mr => new GetPendingQueryResponse
                (
                    mr.Id,
                    mr.User!.FirstName + " " + mr.User.LastName,
                    mr.CreatedAt
                ))
                .ToListAsync(cancellationToken);

            return Result.Success(memberRequests);
        }
    }
}
