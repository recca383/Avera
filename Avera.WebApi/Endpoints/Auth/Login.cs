
namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class Login : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/auth/login", () =>
            {

            })
           . WithTags(Tags.Auth)
            ;
        }
    }
}
