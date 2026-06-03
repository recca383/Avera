
using Avera.Application.Abstractions.Messaging;
using Avera.Application.UpdateCase;
using Avera.Domain.Application.Cases;
using Avera.WebApi.Extensions;
using Avera.WebApi.Infrastructure;

namespace Avera.WebApi.Endpoints.Cases
{
    internal sealed class Update : IEndpoint
    {
        private record UpdateCaseRequest(
            string SubjectName,
            AnalysisType AnalysisType,
            Priority Priority
        );

        public void MapEndpoint(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPut("cases/{id:guid}", async(
                Guid id,
                    UpdateCaseRequest request,
                    ICommandHandler<UpdateCaseCommand, Guid> sender
            )=>
            {
                var command = new UpdateCaseCommand(
                    Id: id,
                    SubjectName: request.SubjectName,
                    AnalysisType: request.AnalysisType,
                    Priority: request.Priority
                );

                var result = await sender.Handle(command, CancellationToken.None);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithTags(Tags.Cases)
            .WithSummary("Update an existing case")
            ;
        }
    }
}