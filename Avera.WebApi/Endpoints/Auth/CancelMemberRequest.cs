using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.ChangeEmail;
using Avera.Application.MemberRequests.Cancel;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;

namespace Avera.WebApi.Endpoints.Auth
{
    internal sealed class CancelMemberRequest : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("auth/cancel-member-request", async (
                ICommandHandler<CancelMemberRequestCommand> handler,
                CancellationToken cancellationToken
            ) =>
            {
                var command = new CancelMemberRequestCommand();

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Auth);
        }
    }
}
