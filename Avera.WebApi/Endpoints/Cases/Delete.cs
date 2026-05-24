
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Delete;
using Avera.WebApi.Infrastructure;
using Avera.WebApi.Extensions;

namespace Avera.WebApi.Endpoints.Cases
{
    internal sealed class Delete : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapDelete("cases/{id:guid}", async(
                Guid id,
                ICommandHandler<DeleteCaseCommand> handler,
                CancellationToken cancellationToken
            )=>
            {
                var command = new DeleteCaseCommand(id);

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
           .WithTags(Tags.Cases)
           .WithName("Delete")
            ;
        }
    }
}