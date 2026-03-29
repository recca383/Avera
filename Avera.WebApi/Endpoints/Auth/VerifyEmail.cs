
namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class VerifyEmail : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/auth/verify-email", ()=>
            {

            });
        }
    }
}
