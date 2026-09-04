using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.UpdateCase;
using Avera.Domain.Application.Cases;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Avera.Application.Cases.UpdateCase
{
    internal sealed class UpdateCaseCommandHandler(
        IApplicationDbContext applicationDbContext,
        //Temporary Logger
        ILogger<UpdateCaseCommandHandler> logger) : ICommandHandler<UpdateCaseCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(UpdateCaseCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling UpdateCaseCommand for Case ID: {CaseId}", command.Id);
            var selectedCase = applicationDbContext.Cases.FirstOrDefault(c => c.Id == command.Id);

            if (selectedCase is null)
            {
                logger.LogWarning("Case not found with ID: {CaseId}", command.Id);
                return await Task.FromResult(Result.Failure<Guid>(CaseErrors.CaseNotFound));
            }

            selectedCase.SubjectName = command.SubjectName;
            selectedCase.AnalysisType = command.AnalysisType;
            selectedCase.Priority = command.Priority;

            applicationDbContext.Cases.Update(selectedCase);
            await applicationDbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Case updated successfully with ID: {CaseId}", command.Id);
            return Result.Success(command.Id);
        }
    }
}