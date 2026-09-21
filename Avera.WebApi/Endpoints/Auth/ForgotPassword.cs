
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.Common;
using Avera.Application.Authentication.ForgotPassword;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class ForgotPassword : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost(
                "/auth/forgot-password",
                async (
                    [FromBody] ForgotPasswordCommand command,
                    ICommandHandler<ForgotPasswordCommand, TokenExpiryResponse> handler,
                    CancellationToken cancellationToken) =>
                {
                    var result = await handler.Handle(
                        command,
                        cancellationToken);

                    return result.Match(
                        Results.Ok,
                        CustomResults.Problem);
                })
                .WithTags(Tags.Auth);
        }
    }
}
