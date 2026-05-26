using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Abstractions.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Avera.Application.CaseImages.DeleteReference
{
    internal sealed class DeleteReferenceCaseImageCommandHandler(
        IApplicationDbContext applicationDbContext,
        IBlobStorageService blobStorageService,
        ILogger<DeleteReferenceCaseImageCommandHandler> logger
    ) : ICommandHandler<DeleteReferenceCaseImageCommand>
    {
        public async Task<Result> Handle(DeleteReferenceCaseImageCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling DeleteReferenceCaseImageCommand for CaseId: {CaseId} and Index: {Index}", command.CaseId, command.Index);
            var selectedCase = await applicationDbContext.Cases
                .Include(c => c.CaseImages)
                .FirstOrDefaultAsync(c => c.Id == command.CaseId, cancellationToken);
            
            var CaseImageToDelete = selectedCase?.CaseImages
                .FirstOrDefault(ci => ci.Type == Domain.Application.CaseImages.ImageType.Reference 
                                && ci.Index == command.Index);

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