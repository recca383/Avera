
using Avera.Application.Abstractions.Messaging;
using Avera.Application.CaseImages.DeleteSuspected;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Avera.WebApi.Endpoints.CaseImages
{
    internal sealed class DeleteSuspected : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapDelete("cases/{caseId:guid}/signatures/suspected/{index:int}", 
            async (
                Guid caseId,
                int index,
                [FromServices] ICommandHandler<DeleteSuspectedCaseImageCommand> handler,
                CancellationToken cancellationToken
            )=>
            {
                var command = new DeleteSuspectedCaseImageCommand(
                    CaseId: caseId,
                    Index: index
                );

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.CaseImages)
            .WithSummary("Delete a suspected image from a case");
        }
    }
}