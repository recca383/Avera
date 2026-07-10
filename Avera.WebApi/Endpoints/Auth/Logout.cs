
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.Logout;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;

namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class Logout : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/auth/logout", async (
                LogoutCommand command,
                ICommandHandler<LogoutCommand> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
           .WithTags(Tags.Auth)
           .RequireAuthorization();
        }
    }
}
