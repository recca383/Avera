
namespace Avera.WebApi.Endpoints.ML
{
    internal sealed class ViewResults : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("ml/results/{caseId:guid}", ()=>
            {
                
            })
            .WithTags(Tags.ML);
        }
    }
}