using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.ChangeEmail;
using Avera.Application.Authentication.Common;
using Avera.Application.Users.ChangeName;
using Avera.WebApi.Endpoints;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Cases
{
    internal class ChangeName : IEndpoint
    {
        private sealed record Request(
            string? NewName,
            string? NewLastName
        );

        public void MapEndpoint(
            IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/auth/change-name", async (
                    Request request,
                    ICommandHandler<ChangeNameCommand> handler,
                    CancellationToken cancellationToken) =>
            {
                var command =
                    new ChangeNameCommand(
                        request.NewName,
                        request.NewLastName);

                var result = await handler.Handle(
                    command,
                    cancellationToken);

                return result.Match( 
                    Results.NoContent,
                    CustomResults.Problem);
            })
                .RequireAuthorization()
                .WithTags(Tags.Auth);
        }
        
    }
}
