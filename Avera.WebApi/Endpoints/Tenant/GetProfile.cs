using Avera.Application.Abstractions.Messaging;
using Avera.Application.Tenants.GetProfile;
using Avera.Domain.Identity.Tenants;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;

namespace Avera.WebApi.Endpoints.Tenant
{
    internal sealed class GetProfile : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("/tenant/profile", async (
                IQueryHandler<GetTenantProfileQuery, TenantProfileDto> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var result = await handler.Handle(
                    new GetTenantProfileQuery(),
                    cancellationToken);

                return result.Match(
                    Results.Ok,
                    CustomResults.Problem);
            })
            .RequireAuthorization()
            .WithTags("Tenant");
        }
    }
}