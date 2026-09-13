
using Avera.Application.Cases.Get;
using Avera.Domain.Application.Cases;
using Avera.WebApi.Infrastructure;
using Avera.WebApi.Extensions;
using SharedKernel;
using Microsoft.AspNetCore.Mvc;
using Avera.Application.Abstractions.Messaging;

namespace Avera.WebApi.Endpoints.Cases
{
    internal sealed class Get : IEndpoint
    {
        private record GetCasesRequest(
            Status? CaseStatus,
            Priority? AnalysisPriority,
            DocumentType? AnalysisType,
            int? Page,
            int? PageSize
        );
    
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("cases", async (
                [AsParameters] GetCasesRequest request,
                [FromServices]IQueryHandler<GetCasesQuery, GetCasesQueryResult> handler,
                CancellationToken cancellationToken
            )=>
            {
                var query = new GetCasesQuery(
                    request.CaseStatus,
                    request.AnalysisPriority,
                    request.AnalysisType,
                    request.Page ?? 1,
                    request.PageSize ?? 10
                );

                Result<GetCasesQueryResult>? result = await handler.Handle(query, cancellationToken);


                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Cases)
            .WithSummary("Get all cases")

            ;
        }

        
    }
}