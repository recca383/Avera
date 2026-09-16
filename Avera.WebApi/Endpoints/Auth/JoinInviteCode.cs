using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.JoinInviteCode;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class JoinInviteCode : IEndpoint
    {
        private sealed record Request(
            string InviteCode
            );
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/auth/join-invite-code", async (
                 Request request,
                ICommandHandler<JoinInviteCodeCommand> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new JoinInviteCodeCommand(request.InviteCode);

                var results = await handler.Handle(command, cancellationToken);

                return results.Match(Results.Created, CustomResults.Problem);
            })
            .WithTags(Tags.Auth)
            .RequireAuthorization();
        }
    }
}