
namespace Avera.WebApi.Endpoints.Cases
{
    internal sealed class Delete : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapDelete("cases/{id:guid}", ()=>
            {
                
            })
           . WithTags(Tags.Cases)
            ;
        }
    }
}