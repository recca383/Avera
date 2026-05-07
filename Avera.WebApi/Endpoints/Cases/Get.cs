
namespace Avera.WebApi.Endpoints.Cases
{
    internal sealed class Get : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("cases", ()=>
            {
                
            })
           . WithTags(Tags.Cases)
            ;
        }
    }
}