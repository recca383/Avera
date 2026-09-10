using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.VerifyPasswordCode;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class VerifyPasswordCode : IEndpoint
    {
        public sealed record Request(
            string Email,
            string Code
        );

        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/auth/password/verify-code", async (
                Request request,
                ICommandHandler<VerifyPasswordCodeCommand> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new VerifyPasswordCodeCommand(
                    request.Email,
                    request.Code);

                var result = await handler.Handle(
                    command,
                    cancellationToken);

                return result.Match(
                    Results.NoContent,
                    CustomResults.Problem);
            })
            .WithTags(Tags.Auth);
        }
    }
}