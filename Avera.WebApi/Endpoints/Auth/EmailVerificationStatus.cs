using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.Common;
using Avera.Application.Authentication.EmailVerificationStatus;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace Avera.WebApi.Endpoints.Auth;

internal sealed class EmailVerificationStatus : IEndpoint
{
    public void MapEndpoint(
        IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet(
            "/auth/email-verification-status",
            async (
                IQueryHandler<
                    GetEmailVerificationStatusQuery,
                    EmailVerificationStatusResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new GetEmailVerificationStatusQuery();
                var result = await handler.Handle(
                    query,
                    cancellationToken);

                return result.Match(
                    Results.Ok,
                    CustomResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Auth);
    }
}