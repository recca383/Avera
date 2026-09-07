using Avera.Application.Abstractions.Messaging;
using Avera.Application.MemberRequests.Approve;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Avera.WebApi.Endpoints.Admin.MemberRequests
{
    public sealed class ApproveMemberRequest : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("tenants/member-requests/{memberrequestid:guid}/approve", async (
                [FromRoute] Guid MemberRequestId,
                [FromServices] ICommandHandler<ApproveCommand> handler,
                CancellationToken cancellationToken
                ) =>
            {
                var command = new ApproveCommand(MemberRequestId);

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .WithTags(Tags.OrgAdmin)
            .RequireAuthorization(RolePolicy.Admin);
        }
    }
}
