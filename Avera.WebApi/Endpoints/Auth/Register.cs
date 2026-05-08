
namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class Register : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/auth/register", () =>
            {

            })
           . WithTags(Tags.Auth)
            ;
        }
    }
}
