
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Abstractions.ML.Health;
using Avera.Application.ML.Health;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;

namespace Avera.WebApi.Endpoints.ML
{
    internal sealed class Health : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("ml/health", async (ICommandHandler<GetMLHealthCommand, GetMLHealthResponse> handler) =>
            {
                var result = await handler.Handle(new GetMLHealthCommand(), CancellationToken.None);

                return result.Match(Results.Ok,CustomResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.ML)
            .WithSummary("Get the health status of the ML service");
        }
    }
}