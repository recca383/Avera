using Avera.Application.Abstractions.Messaging;
using Avera.Application.Tenants.Unsuspend;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Avera.WebApi.Endpoints.Admin;

internal sealed class UnsuspendUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("/admin/unsuspend-user", async (
            [FromBody] Guid UserId,
            [FromServices] ICommandHandler<UnsuspendUserCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new UnsuspendUserCommand(UserId), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .RequireAuthorization(RolePolicy.Admin)
        .WithTags(Tags.OrgAdmin);
    }
}
