namespace Avera.WebApi.Endpoints.Admin
{
    internal sealed class SendInviteCode : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/admin/get-invite-code", async (
                CancellationToken cancellationToken
            ) =>
            {
                
            })
            .WithTags(Tags.OrgAdmin)
            .RequireAuthorization(RolePolicy.Admin);
        }
    }
}