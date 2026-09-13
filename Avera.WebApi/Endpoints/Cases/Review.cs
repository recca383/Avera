using Avera.Application.Abstractions.Messaging;
using Avera.Application.Cases.Review;
using Avera.Domain.Cases;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;

namespace Avera.WebApi.Endpoints.Cases
{
    internal sealed class Review : IEndpoint
    {
        public record Request(
            FinalVerdict FinalVerdict,
            string? ReviewNote,
            bool IsPdfExportAllowed
            );
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("/cases/{Id:guid}/review", async (
                Guid Id,
                Request request,
                ICommandHandler<ReviewCaseCommand> handler,
                CancellationToken cancellationToken
                ) =>
            {
                var command = new ReviewCaseCommand(Id, request.FinalVerdict, request.ReviewNote, request.IsPdfExportAllowed);

                var results = await handler.Handle(command, cancellationToken);

                return results.Match(Results.NoContent, CustomResults.Problem);
            }).RequireAuthorization(RolePolicy.Admin)
            .WithTags(Tags.Cases);

        }
    }
}
