using Avera.Application.Abstractions.Messaging;
using Avera.Application.Tenants.Create;
using Avera.Application.Tenants.Rename;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Avera.WebApi.Endpoints.Admin
{
    internal sealed class Rename : IEndpoint
    {
        private sealed record Request(
            string NewName
            );
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("tenants/rename", async (
                Request request,
                [FromServices] ICommandHandler<RenameTenantCommand> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new RenameTenantCommand(request.NewName);

                var results = await handler.Handle(command, cancellationToken);

                return results.Match(Results.NoContent, CustomResults.Problem);
            })
            .WithTags(Tags.OrgAdmin)
            .RequireAuthorization(RolePolicy.Admin);
        }
    }
}
