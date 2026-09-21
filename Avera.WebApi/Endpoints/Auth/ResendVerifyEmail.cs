using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.Common;
using Avera.Application.Authentication.ResendVerificationEmail;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;

namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class ResendVerifyEmail : IEndpoint
    {
        public sealed record Request(
            string Email,
            string? Type,
            string? NewEmail
        );

        public void MapEndpoint(
            IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost(
                "/auth/resend-verification-email",
                async (
                    Request request,
                    HttpContext httpContext,
                    ICommandHandler<ResendVerificationEmailCommand, TokenExpiryResponse> handler,
                    CancellationToken cancellationToken) =>
                {
                    if (request.Type == "change-email" &&
                        httpContext.User.Identity?.IsAuthenticated != true)
                    {
                        return Results.Unauthorized();
                    }

                    var command =
                        new ResendVerificationEmailCommand(
                            request.Email,
                            request.Type,
                            request.NewEmail);

                    var result = await handler.Handle(
                        command,
                        cancellationToken);

                    return result.Match(
                        Results.Ok,
                        CustomResults.Problem);
                })
                .RequireRateLimiting("resend-verification")
                .WithTags(Tags.Auth);
        }
    }
}
