
using Avera.Application.Abstractions.Messaging;
using Avera.Application.CaseImages.UploadReference;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Avera.WebApi.Endpoints.CaseImages
{
    internal sealed class UploadReference : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("cases/{caseId:guid}/signatures/reference", 
            async (
                Guid caseId,
                IFormFile file,
                int index,
                [FromServices] ICommandHandler<UploadReferenceCaseImageCommand, Guid> handler,
                CancellationToken cancellationToken
            )=>
            {
                var fileStream = file.OpenReadStream();

                var command = new UploadReferenceCaseImageCommand(
                    CaseId: caseId,
                    File: fileStream,
                    MimeType: file.ContentType,
                    Index: index,
                    Size: file.Length
                );

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .DisableAntiforgery()
            .RequireAuthorization()
            .WithTags(Tags.CaseImages)
            .WithSummary("Upload a reference image for a case"); // Development Purpose Only - Remove Before Production;
            
        }
    }
}