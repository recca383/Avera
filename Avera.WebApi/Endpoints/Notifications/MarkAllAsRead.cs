
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Notifications.Get;
using Avera.Application.Notifications.MarkAllAsRead;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Avera.WebApi.Endpoints.Notifications
{
    internal sealed class MarkAllAsRead : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPatch("notifications/read-all", async (
                [FromServices] ICommandHandler<MarkAllNotificationsAsReadCommand> handler,
                CancellationToken cancellationToken)=>
            {
                var command = new MarkAllNotificationsAsReadCommand();

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .WithTags(Tags.Notifications)
            .RequireAuthorization();
        }
    }
}