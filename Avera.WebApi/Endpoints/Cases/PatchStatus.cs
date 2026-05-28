
using Avera.Application.Abstractions.Messaging;
using Avera.Application.PatchStatus;
using Avera.Domain.Application.Cases;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;

namespace Avera.WebApi.Endpoints.Cases
{
    internal sealed class PatchStatus : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPatch("cases/{id:guid}/status", async (
                Guid id, 
                Status status, 
                ICommandHandler<PatchStatusCaseCommand, Guid> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new PatchStatusCaseCommand
                {
                    Id = id,
                    Status = status
                };

                var result = await handler.Handle(command, cancellationToken);
                
                return result.Match(Results.Ok, CustomResults.Problem);
            })
           . WithTags(Tags.Cases)
            ;
        }
    }
}