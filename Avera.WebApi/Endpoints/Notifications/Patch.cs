
namespace Avera.WebApi.Endpoints.Notifications
{
    internal sealed class Patch : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("notifications/read-all", ()=>
            {
                
            })
            .WithTags(Tags.Notifications);
        }
    }
}