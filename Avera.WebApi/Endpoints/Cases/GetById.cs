
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Cases.GetById;
using Avera.WebApi.Infrastructure;
using Avera.WebApi.Extensions;
namespace Avera.WebApi.Endpoints.Cases
{
    internal sealed class GetById : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("cases/{id:guid}", async(
                Guid id,
                IQueryHandler<GetCaseByIdQuery, GetCaseByIdQueryResult> handler,
                CancellationToken cancellationToken
            )=>
            {
                var query = new GetCaseByIdQuery(id);

                var result = await handler.Handle(query, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
           . WithTags(Tags.Cases)
           .WithName("Get By Id")
            ;
        }
    }
}