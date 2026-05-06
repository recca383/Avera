
namespace Avera.WebApi.Endpoints.Cases
{
    internal sealed class RunAnalysis : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("cases/{id:guid}/run-analysis", ()=>
            {
                
            })
           . WithTags(Tags.Cases)
            ;
        }
    }
}