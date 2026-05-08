
namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class RegisterAdmin : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/auth/register-admin", () =>
            {

            })
           . WithTags(Tags.Auth)
            ;
        }
    }
}
