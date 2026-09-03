using Avera.Application.Abstractions.Messaging;
using Avera.Application.Tenants.GetMembers;
using Avera.Domain.Identity.Tenants;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Avera.WebApi.Endpoints.Admin
{
    public sealed class GetMembers : IEndpoint
    {
        public record SearchParameters
        {
            [FromQuery(Name = "IsAlphabetical")]
            public bool? IsAlphabetical { get; init; }

            [FromQuery(Name = "IsMostCases")]
            public bool? IsMostCases { get; init; }

            [FromQuery(Name = "Name")]
            public string? Name { get; init; }
        }

        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("tenant/members", async (
                [AsParameters] SearchParameters searchParameters,
                [FromServices] IQueryHandler<GetMembersQuery, List<TenantMemberDto>> handler,
                CancellationToken cancellationToken
            ) =>
            { 
                var query = new GetMembersQuery
                (
                    searchParameters.IsAlphabetical,
                    searchParameters.IsMostCases,
                    searchParameters.Name
                );

                var result = await handler.Handle(query, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithTags(Tags.OrgAdmin)
            .RequireAuthorization(RolePolicy.Admin);
        }
    }
}