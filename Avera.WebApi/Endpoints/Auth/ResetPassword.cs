
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.ResetPassword;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;

namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class ResetPassword : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/auth/reset-password", async (
                ResetPasswordCommand command,
                ICommandHandler<ResetPasswordCommand> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
           . WithTags(Tags.Auth)
            ;
        }
    }
}
