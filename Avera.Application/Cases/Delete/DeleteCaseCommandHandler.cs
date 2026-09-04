using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Abstractions.Storage;
using Avera.Application.Delete;
using Avera.Domain.Application.Cases;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SharedKernel;

namespace Avera.Application.Cases.Delete
{
    internal sealed class DeleteCaseCommandHandler(
        IApplicationDbContext dbContext,
        IBlobStorageService blobStorageService,
        IUserContext userContext,
        UserManager<User> userManager) : ICommandHandler<DeleteCaseCommand>
    {

        private static readonly ILogger logger = Log.ForContext<DeleteCaseCommandHandler>();
        public async Task<Result> Handle(DeleteCaseCommand command, CancellationToken cancellationToken)
        {
            if (userContext.TenantId == null)
                return Result.Failure<Case>(TenantErrors.NotMember);

            var user = await userManager.FindByIdAsync(userContext.UserId.ToString());

            if (user!.IsSuspended)
                return Result.Failure<Case>(UserErrors.IsSuspended);

            var caseToDelete = await dbContext.Cases.FirstOrDefaultAsync(c => c.Id == command.CaseId, cancellationToken);

            logger.Information("Attempting to delete case with ID: {CaseId}", command.CaseId);

            if(caseToDelete is null)
            {
                logger.Warning("Case with ID: {CaseId} not found", command.CaseId);
                return Result.Failure(CaseErrors.CaseNotFound);
            }

            logger.Information("Deleting blob storage for case with code: {CaseCode}", caseToDelete!.CaseCode);

            await blobStorageService.DeleteFolderAsync(caseToDelete!.CaseCode, cancellationToken);

            logger.Information("Deleted blob storage for case with code: {CaseCode}", caseToDelete.CaseCode);


            logger.Information("Removing case with ID: {CaseId} from database", caseToDelete.Id);

            dbContext.Cases.Remove(caseToDelete!);

            dbContext.CaseImages.Where(c => c.CaseId == caseToDelete.Id).ExecuteDelete();

            logger.Information("Removed case with ID: {CaseId} from database", caseToDelete.Id);

            await dbContext.SaveChangesAsync(cancellationToken);

            logger.Information("Case deleted successfully with ID: {CaseId}", command.CaseId);
            return Result.Success();
        }
    }
}