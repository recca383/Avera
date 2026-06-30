
using Avera.Application.Abstractions.Messaging;
using Avera.Application.ML.GetResults;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using SharedKernel;

namespace Avera.WebApi.Endpoints.ML
{
    internal sealed class GetResults : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("cases/{caseId:guid}/results", async (
                Guid caseId,
                ICommandHandler<GetMLResultsCommand, GetMLResultsResponse> handler,
                CancellationToken cancellationToken
            )=>
            {
                var command = new GetMLResultsCommand(caseId);

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(
                    onSuccess => Results.Stream(result.Value.ResultsStream, "application/pdf"),
                    onFailure: CustomResults.Problem);
            })
            .WithTags(Tags.ML)
            .WithSummary("Get the results of an ML analysis for a case");
        }
    }
}