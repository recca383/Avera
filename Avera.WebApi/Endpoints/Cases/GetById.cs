
namespace Avera.WebApi.Endpoints.Cases
{
    internal sealed class GetById : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("cases/{id:guid}", ()=>
            {
                
            })
           . WithTags(Tags.Cases)
            ;
        }
    }
}