using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.ChangeEmail;
using Avera.Application.Authentication.VerifyEmail;
using Avera.Infrastructure.Services;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Razor.Templating.Core;
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

                var html = await RazorTemplateEngine.RenderAsync("ErrorPages/email-verified.cshtml");

                return result.Match(_ => Results.Redirect(result.Value), _ => Results.Content(html, "text/html"));
            })
            .WithTags(Tags.Auth)
            .AllowAnonymous();
        }
        private static string GetTemplatePath(string templateName)
        {
            var assemblyLocation =
                Path.GetDirectoryName(
                    typeof(EmailService).Assembly.Location)!;

            return Path.Combine(
                assemblyLocation,
                "EmailTemplates",
                templateName);
        }
    }
}