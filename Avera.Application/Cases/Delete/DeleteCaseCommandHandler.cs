using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Abstractions.Storage;
using Avera.Application.Delete;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Avera.Application.Cases.Delete
{
    internal sealed class DeleteCaseCommandHandler(
        IApplicationDbContext dbContext,
        IBlobStorageService blobStorageService,
        ILogger<DeleteCaseCommandHandler> logger) : ICommandHandler<DeleteCaseCommand>
    {
        public async Task<Result> Handle(DeleteCaseCommand command, CancellationToken cancellationToken)
        {
            var caseToDelete = await dbContext.Cases.FirstOrDefaultAsync(c => c.Id == command.CaseId, cancellationToken);

            logger.LogInformation("Attempting to delete case with ID: {CaseId}", command.CaseId);
            // if (caseToDelete is null)
            // {
            //     return Task.FromResult(Result.Failure("Case not found."));
            // }

            logger.LogInformation("Deleting blob storage for case with code: {CaseCode}", caseToDelete!.CaseCode);
            await blobStorageService.DeleteAsync(caseToDelete!.CaseCode, cancellationToken);
            logger.LogInformation("Deleted blob storage for case with code: {CaseCode}", caseToDelete.CaseCode);

            logger.LogInformation("Removing case with ID: {CaseId} from database", caseToDelete.Id);
            dbContext.Cases.Remove(caseToDelete!);
            logger.LogInformation("Removed case with ID: {CaseId} from database", caseToDelete.Id);

            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Case deleted successfully with ID: {CaseId}", command.CaseId);
            return Result.Success();
        }
    }
}