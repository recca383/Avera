
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.Register;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;

namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class Register : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/auth/register", async (
                RegisterCommand command,
                ICommandHandler<RegisterCommand> handler,
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
