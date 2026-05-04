
namespace Avera.WebApi.Endpoints.ML
{
    internal sealed class Results : IEndpoint
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