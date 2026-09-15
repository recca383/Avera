
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Cases.Create;
using Avera.Domain.Application.Cases;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;

namespace Avera.WebApi.Endpoints.Cases
{
    internal sealed class Create : IEndpoint
    {
        private sealed record Request(
            string SubjectName,
            Priority Priority,
            DocumentType DocumentType
        );
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("cases", async(
                Request request,
                ICommandHandler<CreateCaseCommand, Case> handler,
                CancellationToken cancellationToken
            )=>
            {
                var command = new CreateCaseCommand(
                    request.SubjectName,
                    request.Priority,
                    request.DocumentType
                );

                var result = await handler.Handle(command, cancellationToken);

                var locationUri = $"cases/{result.Value.Id}";
                return result.Match(
                    onSuccess => Results.Created(locationUri, result.Value),
                    CustomResults.Problem);
            })
            .RequireAuthorization()
           .WithTags(Tags.Cases)
           .WithSummary("Create a new case")
            ;
        }
    }
}