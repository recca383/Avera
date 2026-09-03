using Avera.Application.Abstractions.Messaging;
using Avera.Application.Tenants.RemoveFromTenant;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Npgsql.Replication;

namespace Avera.WebApi.Endpoints.Admin
{
    internal sealed class RemoveUser : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapDelete("/tenant/members/{userId:guid}", async (
                Guid userId,
                ICommandHandler<RemoveUserCommand> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new RemoveUserCommand(userId);

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .RequireAuthorization(RolePolicy.Admin)
            .WithTags(Tags.OrgAdmin);
        }
    }
}
