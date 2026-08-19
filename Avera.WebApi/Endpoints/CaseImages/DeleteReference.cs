
using Avera.Application.Abstractions.Messaging;
using Avera.Application.CaseImages.DeleteReference;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Avera.WebApi.Endpoints.CaseImages
{
    internal sealed class DeleteReference : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapDelete("cases/{caseId:guid}/signatures/reference/{index:int}", 
            async (
                Guid caseId,
                int index,
                [FromServices] ICommandHandler<DeleteReferenceCaseImageCommand> handler,
                CancellationToken cancellationToken
            )=>
            {
                var command = new DeleteReferenceCaseImageCommand(
                    CaseId: caseId,
                    Index: index
                );

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.CaseImages)
            .WithSummary("Delete a reference image from a case");
        }
    }
}