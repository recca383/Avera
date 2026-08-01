namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class SendInviteCode : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/auth/send-invite-code", async (
                CancellationToken cancellationToken
            ) =>
            {
                
            })
            .WithTags(Tags.Auth)
            .RequireAuthorization();
        }
    }
}