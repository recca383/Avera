using Avera.Application.Abstractions.Messaging;
using Avera.Application.Tenants.SuspendUser;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;

namespace Avera.WebApi.Endpoints.Admin
{
    internal sealed class SuspendUser : IEndpoint
    {
        void IEndpoint.MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/admin/suspend-user", async (
                Guid UserId,
                ICommandHandler<SuspendUserCommand> handler,
                CancellationToken cancellationToken
                ) =>
            {
                var result = await handler.Handle(new SuspendUserCommand(UserId), cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            });
        }
    }
}
