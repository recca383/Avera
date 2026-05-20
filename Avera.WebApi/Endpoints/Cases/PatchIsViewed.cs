
namespace Avera.WebApi.Endpoints.Cases
{
    internal sealed class PatchIsViewed : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPatch("cases/{id:guid}/result-viewed", ()=>
            {
                
            })
           . WithTags(Tags.Cases)
            ;
        }
    }
}