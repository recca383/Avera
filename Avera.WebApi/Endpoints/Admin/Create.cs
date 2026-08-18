using Avera.Application.Abstractions.Messaging;
using Avera.Application.Tenants.Create;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Avera.WebApi.Endpoints.Admin
{
    public sealed class Create : IEndpoint
    {
        public record Request(
            string Name
        );
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("tenants", async (
                Request request,
                [FromServices] ICommandHandler<CreateTenantCommand, Guid> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new CreateTenantCommand(request.Name);

                var results = await handler.Handle(command, cancellationToken);

                return results.Match(Results.Created, CustomResults.Problem);
            })
            .WithTags(Tags.OrgAdmin)
            .RequireAuthorization(RolePolicy.Admin);
        }
    }
}