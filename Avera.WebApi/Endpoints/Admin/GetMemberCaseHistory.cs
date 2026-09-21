using Avera.Application.Abstractions.Messaging;
using Avera.Application.Tenants.GetMemberCaseHistory;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;

namespace Avera.WebApi.Endpoints.Admin;

internal sealed class GetMemberCaseHistory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("tenant/members/{userId:guid}/case-history", async (
            Guid userId,
            IQueryHandler<GetMemberCaseHistoryQuery, List<MemberCaseHistoryItem>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(
                new GetMemberCaseHistoryQuery(userId),
                cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.OrgAdmin)
        .RequireAuthorization(RolePolicy.Admin);
    }
}
