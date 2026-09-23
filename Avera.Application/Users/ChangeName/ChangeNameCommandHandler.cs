using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Users.ChangeName
{
    internal sealed class ChangeNameCommandHandler(
        IUserContext userContext,
        UserManager<User> userManager) : ICommandHandler<ChangeNameCommand>
    {
        public async Task<Result> Handle(ChangeNameCommand command, CancellationToken cancellationToken)
        {
            if (userContext.TenantId == null)
                return Result.Failure(TenantErrors.NotMember);

            var user = await userManager.FindByIdAsync(userContext.UserId.ToString());

            if (user!.IsSuspended)
                return Result.Failure(UserErrors.IsSuspended);

            if (command.NewName != null)
            {
                user.FirstName = command.NewName;
            }

            if (command.NewLastName != null)
            {
                user.LastName = command.NewLastName;
            }

            await userManager.UpdateAsync(user);

            return Result.Success();
        }
    }
}
