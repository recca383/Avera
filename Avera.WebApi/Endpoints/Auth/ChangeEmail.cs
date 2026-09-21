using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.ChangeEmail;
using Avera.Application.Authentication.Common;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using SharedKernel;

namespace Avera.WebApi.Endpoints.Auth
{
    public sealed class ChangeEmail : IEndpoint
    {
        private sealed record Request(
         string NewEmail,
         string CurrentPassword
     );

        public void MapEndpoint(
            IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/auth/change-email", async (
                    Request request,
                    ICommandHandler<ChangeEmailCommand,TokenExpiryResponse> handler,
                    CancellationToken cancellationToken) =>
                {
                    var command =
                        new ChangeEmailCommand(
                            request.NewEmail,
                            request.CurrentPassword);

                    var result = await handler.Handle(
                        command,
                        cancellationToken);

                    return result.Match(
                        Results.Ok,
                        CustomResults.Problem);
                })
                .RequireAuthorization()
                .WithTags(Tags.Auth);
        }
    }
}