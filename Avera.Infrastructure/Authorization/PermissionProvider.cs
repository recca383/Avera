using Avera.Application.Abstractions.Databases;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Avera.Infrastructure.Authorization
{
    internal sealed class PermissionProvider(UserManager<User> userManager)
    {
         public async Task<IList<string>> GetRoleWithPermission(Guid userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if(user is null)
            {
                throw new Exception("User not found");
            }

            IList<string> role = await userManager.GetRolesAsync(user!);

            return role;
        }
    }
}