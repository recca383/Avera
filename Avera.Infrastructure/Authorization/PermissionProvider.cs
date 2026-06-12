using Avera.Application.Abstractions.Databases;
using Avera.Infrastructure.Identity.Roles;
using Microsoft.EntityFrameworkCore;

namespace Avera.Infrastructure.Authorization
{
    internal sealed class PermissionProvider(IIdentityDbContext context)
    {
         public async Task<Role> GetRoleWithPermission(Guid userId)
        {
            // var userRole = await context.UserRoles
            //     .FirstOrDefaultAsync(r => r.UserId == userId) 
            //     ?? throw new InvalidOperationException($"Role not found for user with ID {userId}");

            
            // var role = await context.Roles
            //     .FirstOrDefaultAsync(r => r.Id == userRole.RoleId);

            // return role ?? throw new InvalidOperationException($"Role not found with ID {userRole.RoleId}");

            throw new NotImplementedException("GetRoleWithPermission method is not implemented yet.");
        }
    }
}