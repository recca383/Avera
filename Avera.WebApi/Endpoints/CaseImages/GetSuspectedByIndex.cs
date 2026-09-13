
using Avera.Application.Abstractions.Messaging;
using Avera.Application.CaseImages.GetSuspectedByIndex;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Avera.WebApi.Endpoints.CaseImages
{
    internal sealed class GetSuspectedByIndex : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("cases/{caseId:guid}/signatures/suspected/{index:int}",
            async (
                Guid caseId,
                int index,
                [FromServices] IQueryHandler<GetSuspectedByIndexQuery, GetSuspectedByIndexQueryResponse> handler,
                CancellationToken cancellationToken
            )=>
            {
                var query = new GetSuspectedByIndexQuery(
                    CaseId: caseId,
                    Index: index
                );

                var result = await handler.Handle(query, cancellationToken);

                return result.Match(
                    onsuccess => Results.Stream(
                        result.Value.ImageStream,
                        result.Value.ContentType),
                    CustomResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.CaseImages)
            .WithSummary("Get a suspected image by index");
        }
    }
}