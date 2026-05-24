
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
            Guid Examiner,
            Priority Priority,
            AnalysisType AnalysisType
        );
        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("cases", async(
                Request request,
                ICommandHandler<CreateCaseCommand> handler,
                CancellationToken cancellationToken
            )=>
            {
                var command = new CreateCaseCommand(
                    request.SubjectName,
                    request.Examiner,
                    request.Priority,
                    request.AnalysisType
                );

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Created, CustomResults.Problem);
            })
           .WithTags(Tags.Cases)
           .WithName("Create")
            ;
        }
    }
}