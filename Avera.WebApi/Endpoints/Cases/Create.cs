
namespace Avera.WebApi.Endpoints.Cases
{
    internal sealed class Create : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("cases", ()=>
            {
                
            })
           . WithTags(Tags.Cases)
            ;
        }
    }
}