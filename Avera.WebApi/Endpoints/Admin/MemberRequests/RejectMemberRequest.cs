using Avera.Application.Abstractions.Messaging;
using Avera.Application.MemberRequests.Approve;
using Avera.Application.MemberRequests.Reject;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Avera.WebApi.Endpoints.Admin.MemberRequests
{
    internal sealed class RejectMemberRequest : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("tenants/member-requests/{memberrequestid:guid}/reject", async (
                [FromRoute]Guid MemberRequestId,
                [FromServices] ICommandHandler<RejectCommand> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new RejectCommand(MemberRequestId);

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .WithTags(Tags.OrgAdmin)
            .RequireAuthorization(RolePolicy.Admin);
        }
    }
}
