
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.Login;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;

namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class Login : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/auth/login", async (
                LoginCommand command,
                ICommandHandler<LoginCommand, LoginResponse> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
           . WithTags(Tags.Auth)
            ;
        }
    }
}
