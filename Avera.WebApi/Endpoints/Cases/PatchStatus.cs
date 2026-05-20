
namespace Avera.WebApi.Endpoints.Cases
{
    internal sealed class PatchStatus : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPatch("cases/{id:guid}/status", ()=>
            {
                
            })
           . WithTags(Tags.Cases)
            ;
        }
    }
}