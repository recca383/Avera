using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Users.GetProfilePicture;
using Avera.Domain.Identity.Users;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class GetProfilePicture : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("/auth/me/profile-picture", async (
                [FromServices] IQueryHandler<GetProfilePictureQuery, GetProfileProfileQueryResponse> handler,
                CancellationToken cancellationToken
                ) =>
            {
                var result = await handler.Handle(new GetProfilePictureQuery(), cancellationToken);


                return result.Match(
                    onSuccess => Results.Stream(result.Value.ImageStream, result.Value.ContentType),
                    CustomResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Auth);
        }
    }
}
