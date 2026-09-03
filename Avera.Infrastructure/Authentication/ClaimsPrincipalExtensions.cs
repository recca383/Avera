using System.Security.Claims;
using Serilog;

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
            string? tenantId = principal?.FindFirstValue("TenantId");

            return Guid.TryParse(tenantId, out Guid parsedTenantId)?
                parsedTenantId :
                throw new ApplicationException("Tenant id is unavailable");
        }

        public static string GetEmail(this ClaimsPrincipal? principal)
        {
            string? email = principal?.FindFirstValue(ClaimTypes.Email);

            return email ?? throw new ApplicationException("Email is unavailable");
        }

        public static bool IsAuthenticated(this ClaimsPrincipal? principal)
        {
            return principal?.Identity?.IsAuthenticated ?? false;
        }

        public static IReadOnlyCollection<string> GetRoles(this ClaimsPrincipal? principal)
        {
            return principal?.FindAll(ClaimTypes.Role)
                .Select(claim => claim.Value)
                .ToList() ?? new List<string>();
        }

        public static string GetSecurityStamp(this ClaimsPrincipal? principal)
        {
            return principal?.FindFirstValue("SecurityStamp")!;
        }
    }
}