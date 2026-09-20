using Avera.Application.Abstractions.Messaging;
using Avera.Application.Cases.Flag;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Avera.WebApi.Endpoints.Cases
{
    internal sealed class ToggleFlag : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("cases/{caseId:guid}/flag", async (
                [FromRoute] Guid caseId,
                [FromBody] ToggleFlagRequest request,
                [FromServices] ICommandHandler<ToggleCaseFlagCommand> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new ToggleCaseFlagCommand(caseId, request.IsFlagged);

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .RequireAuthorization(RolePolicy.Admin)
            .WithTags(Tags.OrgAdmin);
        }

        private sealed record ToggleFlagRequest(bool IsFlagged);
    }
}
