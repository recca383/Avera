using Avera.Application.Hubs;

namespace Avera.WebApi.Endpoints.SignalR
{
    internal sealed class Notification : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapHub<NotificationHub>("/hubs/notification").WithTags("Notification Hub").RequireAuthorization();
        }
    }
}
