using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.PatchIsViewed;
using Avera.Domain.Application.Cases;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using SharedKernel;

namespace Avera.Application.Cases.PatchIsViewed
{
    internal sealed class PatchIsViewedCaseCommandHandler
    (IApplicationDbContext applicationDbContext,
     IUserContext userContext,
     UserManager<User> userManager) : ICommandHandler<PatchIsViewedCaseCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(PatchIsViewedCaseCommand command, CancellationToken cancellationToken)
        {
            // Temporary 

            if (userContext.TenantId == null)
                return Result.Failure<Guid>(TenantErrors.NotMember);

            var user = await userManager.FindByIdAsync(userContext.UserId.ToString());

            if (user!.IsSuspended)
                return Result.Failure<Guid>(UserErrors.IsSuspended);
            var selectedCase = applicationDbContext.Notifications.FirstOrDefault(c => c.Id == command.Id);

            if (selectedCase is null)
            {
                return Result.Failure<Guid>(CaseErrors.CaseNotFound);
            }

            // selectedCase.IsViewed = command.IsViewed;
            // applicationDbContext.Cases.Update(selectedCase);
            // applicationDbContext.SaveChanges();

            return Result.Success(command.Id);
        }
    }
}