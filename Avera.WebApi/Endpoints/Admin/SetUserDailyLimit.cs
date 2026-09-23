using Avera.Application.Abstractions.Messaging;
using Avera.Application.Users.SetDailyLimit;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Avera.WebApi.Endpoints.Admin;

internal sealed class SetUserDailyLimit : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapPost("/admin/set-user-daily-limit", async (
            [FromBody] SetUserDailyLimitRequest request,
            [FromServices] ICommandHandler<SetUserDailyLimitCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new SetUserDailyLimitCommand(request.UserId, request.DailyLimit);
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
            .RequireAuthorization(RolePolicy.Admin);
    }
}

public sealed record SetUserDailyLimitRequest(Guid UserId, int? DailyLimit);
