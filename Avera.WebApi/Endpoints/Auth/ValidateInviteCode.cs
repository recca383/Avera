
namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class ValidateInviteCode : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("/auth/validate-invite-code/{code}",()=>
            {

            });
        }
    }
}
