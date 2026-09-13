
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Notifications.Get;
using Avera.Application.Notifications.MarkAsRead;
using Avera.Domain.Notifications;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Avera.WebApi.Endpoints.Notifications
{
    internal sealed class MarkAsRead : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPatch("notifications/{Id:guid}/read", async (
                [FromRoute] Guid Id,
                [FromServices] ICommandHandler <MarkNotificationAsReadCommand> handler,
                CancellationToken cancellationToken
                )=>
            {
                var command = new MarkNotificationAsReadCommand(Id);

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .WithTags(Tags.Notifications)
            .RequireAuthorization();
        }
    }
}