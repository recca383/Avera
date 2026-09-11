using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Domain.Identity.Users;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Principal;

namespace Avera.WebApi.Endpoints.Auth
{
    public class Profile : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("/auth/me", async (
                IUserContext userContext,
                UserManager<User> userManager,
                IIdentityDbContext identityDbContext,
                CancellationToken cancellationToken
                ) =>
            {
                var user = await userManager.FindByIdAsync(userContext.UserId.ToString());

                if (user is null)
                    return Results.NotFound();

                var organization = user.TenantId.HasValue ?
                    await identityDbContext.Tenants
                        .Where(tenant => tenant.Id == user.TenantId.Value)
                        .Select(tenant => tenant.Name)
                        .FirstOrDefaultAsync(cancellationToken)
                        : null;

                var roles = await userManager.GetRolesAsync(user);
                return Results.Ok(new
                {
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email,
                    role = roles.FirstOrDefault() ?? string.Empty,
                    organization = organization ?? string.Empty,
                    avatarUri = (string?)null
                });
            })
            .RequireAuthorization()
            .WithTags(Tags.Auth);
        }
    }
}
