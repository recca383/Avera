
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
            routeBuilder.MapGet("cases/{caseId:guid}/blob/{folder}/{filename}", async(
                Guid caseId,
                string folder,
                string filename,
                IQueryHandler<GetBlobImageQuery, GetBlobImageResponse> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var query = new GetBlobImageQuery(caseId, folder, filename);

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