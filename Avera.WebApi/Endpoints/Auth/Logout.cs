
namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class Logout : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/auth/logout", () =>
            {

            })
           . WithTags(Tags.Auth)
            ;
        }
    }
}
