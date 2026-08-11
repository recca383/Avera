using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.ChangePassword;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using SharedKernel;

namespace Avera.WebApi.Endpoints.Auth
{
    public sealed class ChangePassword : IEndpoint
    {
        private record Request
        (
            string NewPassword,
            string CurrentPassword
        );

        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("auth/change-password", async (
                Request request,
                ICommandHandler<ChangePasswordCommand> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new ChangePasswordCommand(request.NewPassword, request.CurrentPassword);

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Auth);
        }
    }
}