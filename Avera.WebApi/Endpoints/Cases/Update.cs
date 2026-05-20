
namespace Avera.WebApi.Endpoints.Cases
{
    internal sealed class Update : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPatch("cases/{id:guid}", ()=>
            {
                
            })
           . WithTags(Tags.Cases)
            ;
        }
    }
}