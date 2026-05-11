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

        public Guid TenantId => 
            httpContextAccessor
                .HttpContext?
                .User
                .GetTenantId() ??
                throw new ApplicationException("Tenant context is unavailable");


    }
}