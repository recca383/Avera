
using Avera.Application.Abstractions.Messaging;
using Avera.Application.CaseImages.GetReferenceByIndex;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace Avera.WebApi.Endpoints.CaseImages
{
    internal sealed class GetByIndex : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("cases/{caseId:guid}/signatures/reference/{index:int}", 
            async (
                IAntiforgery antiforgery,
                Guid caseId,
                int index,
                [FromServices] IQueryHandler<GetReferenceByIndexQuery, GetReferenceByIndexQueryResponse> handler,
                CancellationToken cancellationToken
            )=>
            {
                var query = new GetReferenceByIndexQuery(
                    CaseId: caseId,
                    Index: index
                );
                
                var result = await handler.Handle(query, cancellationToken);

                return result.Match(
                    onSuccess => Results.Stream(result.Value.ImageStream, result.Value.ContentType),
                    CustomResults.Problem);
            });
        }
    }
}