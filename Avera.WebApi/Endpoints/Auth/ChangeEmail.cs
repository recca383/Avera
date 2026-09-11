using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.ChangeEmail;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using SharedKernel;

namespace Avera.WebApi.Endpoints.Auth
{
    public sealed class ChangeEmail : IEndpoint
    {
        private record Request
        (
            string NewEmail,
            string CurrentPassword
        );

        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("auth/change-email", async (
                Request request,
                ICommandHandler<ChangeEmailCommand> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new ChangeEmailCommand(request.NewEmail, request.CurrentPassword);

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Auth);
        }
    }
}