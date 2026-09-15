using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Cases.Get;
using Avera.Domain.Application.Cases;
using Avera.Domain.Cases.Events;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Cases.Review
{
    internal sealed class ReviewCaseCommandHandler(
        IUserContext userContext,
        IApplicationDbContext applicationDbContext,
        UserManager<User> userManager,
        IDateTimeProvider dateTime
        ) : ICommandHandler<ReviewCaseCommand>
    {
        public async Task<Result> Handle(ReviewCaseCommand command, CancellationToken cancellationToken)
        {
            if (userContext.TenantId == null)
                return Result.Failure(TenantErrors.NotMember);

            var user = await userManager.FindByIdAsync(userContext.UserId.ToString());

            if (user!.IsSuspended)
                return Result.Failure(UserErrors.IsSuspended);

            var selectedCase = await applicationDbContext
                                        .Cases
                                        .FindAsync([command.CaseId], cancellationToken);

            if (selectedCase == null)
            {
                return Result.Failure(CaseErrors.CaseNotFound);
            }

            if (selectedCase.FinalVerdict != Domain.Cases.FinalVerdict.None)
            {
                return Result.Failure(CaseErrors.CaseAlreadyReviewed);
            }

            selectedCase.Review(
                userContext.UserId,
                command.FinalVerdict,
                command.ReviewNote,
                command.IsPdfExportAllowed,
                dateTime.PhilippineNow
                );

            selectedCase.Raise(new CaseReviewCompletedDomainEvent(
                selectedCase.Id,
                userContext.TenantId.Value,
                userContext.UserId,
                command.FinalVerdict,
                dateTime.PhilippineNow
                ));

            await applicationDbContext.SaveChangesAsync();

            
            return Result.Success();
        }
    }
}
