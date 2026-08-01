namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class JoinInviteCode : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/auth/join-invite-code", async (
                string InviteCode,
                CancellationToken cancellationToken
            ) =>
            {
                
            })
            .WithTags(Tags.Auth)
            .RequireAuthorization();
        }
    }
}