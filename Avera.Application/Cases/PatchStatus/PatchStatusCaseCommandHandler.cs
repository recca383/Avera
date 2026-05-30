using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.PatchStatus;
using Avera.Domain.Application.Cases;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Avera.Application.Cases.PatchStatus
{
    internal sealed class PatchStatusCaseCommandHandler
    (IApplicationDbContext applicationDbContext, ILogger<PatchStatusCaseCommandHandler> logger) : ICommandHandler<PatchStatusCaseCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(PatchStatusCaseCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling PatchStatusCaseCommand for case ID: {CaseId}", command.Id);

            var selectedCase = applicationDbContext.Cases.FirstOrDefault(c => c.Id == command.Id);

            if (selectedCase is null)
            {
                logger.LogWarning("Case not found for ID: {CaseId}", command.Id);
                return await Task.FromResult(Result.Failure<Guid>(CaseErrors.CaseNotFound));
            }

            selectedCase.Status = command.Status;
            applicationDbContext.Cases.Update(selectedCase);
            await applicationDbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Case status updated successfully for ID: {CaseId}", command.Id);
            return Result.Success(command.Id);
        }
    }
}