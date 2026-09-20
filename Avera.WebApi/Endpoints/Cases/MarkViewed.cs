using Avera.Application.Abstractions.Messaging;
using Avera.Application.Cases.View;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Avera.WebApi.Endpoints.Cases;

internal sealed class MarkViewed : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("/cases/{caseId:guid}/viewed", async (
            [FromRoute] Guid caseId,
            [FromServices] ICommandHandler<MarkCaseViewedCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new MarkCaseViewedCommand(caseId), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .RequireAuthorization();
    }
}
