
namespace Avera.WebApi.Endpoints.Notifications
{
    internal sealed class PatchById : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPatch("notifications/{id:guid}/read", ()=>
            {
                
            })
            .WithTags(Tags.Notifications);
        }
    }
}