
namespace Avera.WebApi.Endpoints.ML
{
    internal sealed class Health : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("ml/health", ()=>
            {
                
            })
            .WithTags(Tags.ML);
        }
    }
}