using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.ResendVerificationEmail;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;

namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class ResendVerifyEmail : IEndpoint
    {
        public record Request(
            string email,
            string? type,
            string? newEmail
            );
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("auth/resend-verification-email", async (
                Request request,
                ICommandHandler<ResendVerificationEmailCommand> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new ResendVerificationEmailCommand(request.email, request.type, request.newEmail);

                var results = await handler.Handle(command, cancellationToken);

                return results.Match(Results.NoContent, CustomResults.Problem);
            })
                .WithTags(Tags.Auth);
        }
    }
}
