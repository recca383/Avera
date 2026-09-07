using Avera.Application.Abstractions.Messaging;
using Avera.Application.MemberRequests.GetPending;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Avera.WebApi.Endpoints.Admin.MemberRequests
{
    internal sealed class GetPending : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("tenants/member-requests/pending", async (
                [FromServices] IQueryHandler<GetPendingQuery, List<GetPendingQueryResponse>> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new GetPendingQuery();
                var result = await handler.Handle(command, cancellationToken);
                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .RequireAuthorization(RolePolicy.Admin)
            .WithTags(Tags.OrgAdmin);
        }
    }
}
