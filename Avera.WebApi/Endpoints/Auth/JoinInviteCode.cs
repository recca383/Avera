using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.JoinInviteCode;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;

namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class JoinInviteCode : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/auth/join-invite-code", async (
                string InviteCode,
                ICommandHandler<JoinInviteCodeCommand> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new JoinInviteCodeCommand(InviteCode);

                var results = await handler.Handle(command, cancellationToken);

                return results.Match(Results.Created, CustomResults.Problem);
            })
            .WithTags(Tags.Auth)
            .RequireAuthorization();
        }
    }
}