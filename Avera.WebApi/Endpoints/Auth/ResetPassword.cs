
namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class ResetPassword : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/auth/reset-password", () =>
            {

            })
           . WithTags(Tags.Auth)
            ;
        }
    }
}
