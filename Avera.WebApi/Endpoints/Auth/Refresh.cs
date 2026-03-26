
namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class Refresh : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/auth/refresh", () =>
            {

            });
        }
    }
}
