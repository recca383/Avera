using Avera.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace Avera.Infrastructure.Authentication
{
    public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {

        public Guid UserId => 
            httpContextAccessor
                .HttpContext?
                .User
                .GetUserId() ??
                throw new ApplicationException("User context is unavailable");

        public Guid? TenantId => 
            httpContextAccessor
                .HttpContext?
                .User
                .GetTenantId() ??
                throw new ApplicationException("Tenant context is unavailable");

        public string Email => 
            httpContextAccessor
                .HttpContext?
                .User
                .GetEmail() ??
                throw new ApplicationException("Email context is unavailable");

        public bool IsAuthenticated => 
            httpContextAccessor
                .HttpContext?
                .User
                .IsAuthenticated() ??
                throw new ApplicationException("Authentication context is unavailable");

        public IReadOnlyCollection<string> Roles => 
            httpContextAccessor
                .HttpContext?
                .User
                .GetRoles() ??
                throw new ApplicationException("Roles context is unavailable");

        public string SecurityStamp => 
            httpContextAccessor
                .HttpContext?
                .User
                .GetSecurityStamp() ??
                throw new ApplicationException("Invalid Token, please log in again");
    }
}