
namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class ForgotPassword : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/auth/forgot-password", (

                ) =>
            {
                
            })
           . WithTags(Tags.Auth)
           ;
        }
    }
}
