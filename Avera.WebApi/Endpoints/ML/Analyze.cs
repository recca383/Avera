
using Avera.Application.Abstractions.Messaging;
using Avera.Application.ML.Process;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;

namespace Avera.WebApi.Endpoints.ML
{
    internal sealed class Analyze : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("cases/{caseId:guid}/analysis", async (
                Guid caseId,
                ICommandHandler<ProcessCommand, ProcessResponse> commandHandler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new ProcessCommand(caseId);

                var result = await commandHandler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithTags(Tags.ML)
            .WithSummary("Analyze a case using ML");
        }
    }
}