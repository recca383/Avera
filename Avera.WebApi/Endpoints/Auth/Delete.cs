using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.Delete;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;

namespace Avera.WebApi.Endpoints.Auth
{
    public sealed class Delete : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapDelete("auth/delete", async (
                DeleteCommand command,
                ICommandHandler<DeleteCommand> handler,
                CancellationToken cancellationToken
            )=> 
            {
                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            });
        }
    }
}