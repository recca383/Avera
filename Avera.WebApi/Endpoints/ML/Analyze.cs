
namespace Avera.WebApi.Endpoints.ML
{
    internal sealed class Analyze : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("ml/analyze", ()=>
            {
                
            })
            .WithTags(Tags.ML);
        }
    }
}