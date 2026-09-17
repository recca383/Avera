using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Abstractions.Storage;
using Avera.Domain.Application.CaseImages;
using Avera.Domain.Application.Cases;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Avera.Application.CaseImages.DeleteReference
{
    internal sealed class DeleteReferenceCaseImageCommandHandler(
        IApplicationDbContext applicationDbContext,
        IBlobStorageService blobStorageService,
        // Temporary Logger
        ILogger<DeleteReferenceCaseImageCommandHandler> logger,
        IUserContext userContext,
        UserManager<User> userManager
    ) : ICommandHandler<DeleteReferenceCaseImageCommand>
    {
        public async Task<Result> Handle(DeleteReferenceCaseImageCommand command, CancellationToken cancellationToken)
        {
            if (userContext.TenantId == null)
                return Result.Failure(TenantErrors.NotMember);

            var user = await userManager.FindByIdAsync(userContext.UserId.ToString());

            if (user!.IsSuspended)
                return Result.Failure(UserErrors.IsSuspended);

            logger.LogInformation("Handling DeleteReferenceCaseImageCommand for CaseId: {CaseId} and Index: {Index}", command.CaseId, command.Index);
            var selectedCase = await applicationDbContext.Cases
                .Include(c => c.CaseImages)
                .FirstOrDefaultAsync(c => c.Id == command.CaseId, cancellationToken);
            
            if (selectedCase == null)
            {
                return Result.Failure(CaseErrors.CaseNotFound);
            }

            var CaseImageToDelete = selectedCase?.CaseImages
                .FirstOrDefault(ci => ci.Type == Domain.Application.CaseImages.ImageType.Reference 
                                && ci.Index == command.Index);

            if (CaseImageToDelete == null)
            {
                return Result.Failure(CaseImageErrors.CaseImageNotFound);
            }
            logger.LogInformation("Selected Case with Id: {CaseId} has {NumberOfImages} images. Image to delete: {ImageToDelete}", 
                selectedCase!.Id, selectedCase.CaseImages.Count, CaseImageToDelete != null ? $"Id: {CaseImageToDelete.Id}, FileName: {CaseImageToDelete.FileName}" : "Not Found");

            selectedCase?.CaseImages.Remove(CaseImageToDelete!);
            applicationDbContext.CaseImages.Remove(CaseImageToDelete!);

            await blobStorageService.DeleteAsync(CaseImageToDelete!.BlobName, cancellationToken);

            await applicationDbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Deleted Reference Case Image with Id: {ImageId} for CaseId: {CaseId}", CaseImageToDelete!.Id, command.CaseId);
            return Result.Success();
        }
    }
}