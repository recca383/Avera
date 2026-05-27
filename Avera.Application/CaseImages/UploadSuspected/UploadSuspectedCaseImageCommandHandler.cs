using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Abstractions.Storage;
using Avera.Domain.Application.CaseImages;
using Avera.Domain.Application.Cases;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Avera.Application.CaseImages.UploadSuspected
{
    internal sealed class UploadSuspectedCaseImageCommandHandler
    (IApplicationDbContext applicationDbContext,
    IBlobStorageService blobStorage,
    ILogger<UploadSuspectedCaseImageCommandHandler> logger) : ICommandHandler<UploadSuspectedCaseImageCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(UploadSuspectedCaseImageCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling UploadSuspectedCaseImageCommand for Case with ID {CaseId}", command.CaseId);
            var selectedCase = applicationDbContext.Cases.Find(command.CaseId);

            if (selectedCase is null)
            {
                logger.LogWarning("Case with ID {CaseId} not found", command.CaseId);
                return Result.Failure<Guid>(CaseErrors.CaseNotFound);
            }

            var newImage = new CaseImage
            {
                Id = Guid.NewGuid(),
                CaseId = command.CaseId,
                MimeType = command.MimeType,
                Case = selectedCase,
                Index = command.Index,
                Type = ImageType.Suspected,
                Size = command.Size,
                UploadedAt = DateTime.UtcNow,
            };

            try
            {
                logger.LogInformation("Uploading image for Case with ID {CaseId} to blob storage", command.CaseId);
                await blobStorage.UploadFileAsync(
                    command.File,
                    newImage.BlobName,
                    command.MimeType,
                    cancellationToken
                );

                applicationDbContext.CaseImages.Add(newImage);
                selectedCase.CaseImages.Add(newImage);
                await applicationDbContext.SaveChangesAsync(cancellationToken);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to upload image for Case with ID {CaseId}", command.CaseId);
                return Result.Failure<Guid>(CaseImageErrors.ImageUploadFailed);
            }


            return Result.Success(newImage.Id);
        }
    }
}