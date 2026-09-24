using Avera.Application.Abstractions.Messaging;
using Avera.Application.CaseImages.UploadReference;
using Avera.Application.Users.UploadProfilePicture;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class UploadProfilePicture : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/auth/me/profile-picture",
                async (
                    IFormFile file,
                    [FromServices] ICommandHandler<UploadProfilePictureCommand> handler,
                    CancellationToken cancellationToken
                ) =>
            {
                var fileStream = file.OpenReadStream();

                var command = new UploadProfilePictureCommand(
                    file.ContentType,
                    file.FileName,
                    fileStream
                );

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .DisableAntiforgery()
            .RequireAuthorization()
            .WithTags(Tags.Auth);
        }
    }
}
