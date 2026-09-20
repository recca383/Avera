using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Abstractions.Storage;
using Avera.Domain.Application.CaseImages;
using Avera.Domain.Application.Cases;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Avera.Application.CaseImages.UploadReference
{
    internal sealed class UploadReferenceCaseImageCommandHandler
    (IApplicationDbContext applicationDbContext,
    IBlobStorageService blobStorage,
    ILogger<UploadReferenceCaseImageCommandHandler> logger,
    IUserContext userContext,
    UserManager<User> userManager,
    IDateTimeProvider dateTime) : ICommandHandler<UploadReferenceCaseImageCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(UploadReferenceCaseImageCommand command, CancellationToken cancellationToken)
        {

            if (userContext.TenantId == null)
                return Result.Failure<Guid>(TenantErrors.NotMember);

            var user = await userManager.FindByIdAsync(userContext.UserId.ToString());

            if (user!.IsSuspended)
                return Result.Failure<Guid>(UserErrors.IsSuspended);

            logger.LogInformation("Handling UploadReferenceCaseImageCommand for Case with ID {CaseId}", command.CaseId);
            var selectedCase = applicationDbContext.Cases.Find(command.CaseId);

            if (selectedCase is null)
            {
                logger.LogWarning("Case with ID {CaseId} not found", command.CaseId);
                return Result.Failure<Guid>(CaseErrors.CaseNotFound);
            }

            // If an image already exists for this case/type/index, delete it first (upsert behavior)
            var existingImage = selectedCase.CaseImages.FirstOrDefault(ci => ci.Type == ImageType.Reference && ci.Index == command.Index);

            if (existingImage != null)
            {
                try
                {
                    await blobStorage.DeleteAsync(existingImage.BlobName, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to delete existing blob {Blob} before replacing", existingImage.BlobName);
                }

                selectedCase.CaseImages.Remove(existingImage);
                applicationDbContext.CaseImages.Remove(existingImage);
            }

            var newImage = new CaseImage
            {
                Id = Guid.NewGuid(),
                MimeType = command.MimeType,
                CaseId = command.CaseId,
                Case = selectedCase,
                Index = command.Index,
                Type = ImageType.Reference,
                Size = command.Size,
                UploadedAt = dateTime.PhilippineNow,
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