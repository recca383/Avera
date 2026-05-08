
namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class Google : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("auth/google", () =>
            {

            })
           . WithTags(Tags.Auth)
            ;
        }
    }
}
