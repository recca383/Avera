
namespace Avera.WebApi.Endpoints.Notifications
{
    internal sealed class Get : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("notifications/", ()=>
            {
                
            })
            .WithTags(Tags.Notifications);
        }
    }
}