

using Avera.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Avera.Infrastructure.Authorization
{
    internal sealed class PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    : AuthorizationHandler<PermissionRequirement>
    {
        private readonly static ILogger logger = Log.ForContext<PermissionAuthorizationHandler>();
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            logger.Information("Checking User Permission");
            if (context.User is {Identity.IsAuthenticated:false})
            {
                logger.Warning("User is not authenticated");
                context.Fail();
                return;
            }

            using IServiceScope scope = serviceScopeFactory.CreateScope();

            PermissionProvider permissionProvider = scope.ServiceProvider.GetRequiredService<PermissionProvider>();

            logger.Information("Getting user permission using context");
            var roles = await permissionProvider.GetRoleWithPermission(context.User.GetUserId());

            foreach(string role in roles)
            {
                logger.Information("Comparing policy {policy} to user claim {claim}", requirement.Permission, role);
                if(role.Equals(requirement.Permission, StringComparison.OrdinalIgnoreCase))
                {
                    logger.Information("Comparison succeeded");
                    context.Succeed(requirement);
                    return;
                }
            }
            
            logger.Warning("User claim does not match policy");
            context.Fail();
            
        }
    }
}