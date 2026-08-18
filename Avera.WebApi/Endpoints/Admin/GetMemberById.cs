using Avera.Application.Abstractions.Messaging;
using Avera.Application.Tenants.GetMemberById;
using Avera.Domain.Identity.Tenants;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Avera.WebApi.Endpoints.Admin
{
    public sealed class GetMemberById : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("tenant/member/{userId:guid}", async (
                [FromRoute] Guid userId,
                [FromServices] ICommandHandler<GetMemberByIdCommand, TenantMemberDto> handler,
                CancellationToken cancellationToken
            )=>
            {
                var command = new GetMemberByIdCommand(userId);

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithTags(Tags.OrgAdmin)
            .RequireAuthorization(RolePolicy.Admin);
        }
    }
}