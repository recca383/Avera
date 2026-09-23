using Avera.Application.Abstractions.Messaging;
using Avera.Application.Tenants.SetMemberCountLimit;
using Avera.Application.Users.SetDailyLimit;
using Avera.WebApi.Endpoints.Admin;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.WebApi.Endpoints.Tenant
{
    internal sealed class SetMemberCountLimit : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/admin/set-member-count-limit", async (
            [FromBody] int request,
            [FromServices] ICommandHandler<SetMemberCountLimitCommand> handler,
            CancellationToken cancellationToken) =>
            {
                var command = new SetMemberCountLimitCommand(request);

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
                .RequireAuthorization(RolePolicy.Admin);
        }
    }
}
