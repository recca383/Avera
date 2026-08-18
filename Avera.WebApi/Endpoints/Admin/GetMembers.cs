namespace Avera.WebApi.Endpoints.Admin
{
    public sealed class GetMembers : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("tenant/member", async (
                
            ) =>
            {
                
            })
            .WithTags(Tags.OrgAdmin)
            .RequireAuthorization(RolePolicy.Admin);
        }
    }
}