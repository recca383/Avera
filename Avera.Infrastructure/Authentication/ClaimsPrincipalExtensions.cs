using System.Security.Claims;

namespace Avera.Infrastructure.Authentication
{
    internal static class ClaimsPrincipalExtensions
    {
         public static Guid GetUserId(this ClaimsPrincipal? principal)
        {
            string? userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(userId, out Guid parsedUserId)?
                parsedUserId :
                throw new ApplicationException("User id is unavailable");
        }

        public static Guid GetTenantId(this ClaimsPrincipal? principal)
        {
            string? tenantId = principal?.FindFirstValue(ClaimTypes.GroupSid);

            return Guid.TryParse(tenantId, out Guid parsedTenantId)?
                parsedTenantId :
                throw new ApplicationException("Tenant id is unavailable");
        }
    }
}