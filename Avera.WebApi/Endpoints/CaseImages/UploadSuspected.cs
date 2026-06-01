
using Avera.Application.Abstractions.Messaging;
using Avera.Application.CaseImages.UploadReference;
using Avera.Application.CaseImages.UploadSuspected;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Avera.WebApi.Endpoints.CaseImages
{
    internal sealed class UploadSuspected : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("cases/{caseId:guid}/signatures/suspected",
            async (
                Guid caseId,
                IFormFile file,
                int index,
                [FromServices] ICommandHandler<UploadSuspectedCaseImageCommand, Guid> handler,
                CancellationToken cancellationToken
            )=>
            {

                var fileStream = file.OpenReadStream();

                var command = new UploadSuspectedCaseImageCommand(
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
            .WithTags(Tags.CaseImages)
            .WithSummary("Upload a suspected image for a case"); // Development Purpose Only - Remove Before Production;
        }
    }
}