
using Avera.Application.Abstractions.Messaging;
using Avera.Application.ML.GetBlobImage;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;

namespace Avera.WebApi.Endpoints.ML
{
    internal sealed class GetBlobImage : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("cases/{caseId:guid}/images/{imageId:guid}", async(
                Guid caseId,
                Guid imageId,
                IQueryHandler<GetBlobImageQuery, GetBlobImageResponse> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var query = new GetBlobImageQuery(caseId, imageId);

                var result = await handler.Handle(query, cancellationToken);

                return result.Match(
                    onSuccess => Results.Stream(result.Value.ImageStream, result.Value.ContentType),
                    CustomResults.Problem
                );
            })
            .WithTags(Tags.ML)
            .WithSummary("Get a single gradcam overlay/original image for a case");
        }
    }
}