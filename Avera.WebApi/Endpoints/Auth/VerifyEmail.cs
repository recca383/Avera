using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.ChangeEmail;
using Avera.Application.Authentication.VerifyEmail;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using SharedKernel;

namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class VerifyEmail : IEndpoint
    {
        public sealed record Request(
            Guid UserId,
            string Token,
            string? Type,
            string? Email
        );

        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("/auth/verify-email", async (
                [AsParameters] Request request,
                ICommandHandler<VerifyEmailCommand, string> handler,
                CancellationToken cancellationToken
            ) =>
            {
                
                var command = new VerifyEmailCommand(
                    request.UserId,
                    request.Token,
                    request.Type,
                    request.Email);

                var result = await handler.Handle(
                    command,
                    cancellationToken);

                return result.Match(
                    _ => Results.Redirect(result.Value),
                    CustomResults.Problem);
            })
            .WithTags(Tags.Auth)
            .AllowAnonymous();
        }
    }
}