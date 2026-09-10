using Avera.Application.Abstractions.Messaging;
using Avera.Application.Tenants.GetInviteCode;
using Avera.Domain.Identity.Tenants;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;

namespace Avera.WebApi.Endpoints.Admin
{
    internal sealed class GetInviteCode : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("/tenant/invite-code",
                async (
                    IQueryHandler<
                        GetTenantInviteCodeQuery,
                        TenantInviteCodeDto> handler,
                    CancellationToken cancellationToken) =>
                {
                    var result = await handler.Handle(
                        new GetTenantInviteCodeQuery(),
                        cancellationToken);

                    return result.Match(
                        Results.Ok,
                        CustomResults.Problem);
                })
                .RequireAuthorization(RolePolicy.Admin)
                .WithTags(Tags.OrgAdmin);
        }
    }
}
