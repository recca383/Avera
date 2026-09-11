using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Abstractions.ML;
using Avera.Domain.Application.CaseImages;
using Avera.Domain.Application.Cases;
using Avera.Domain.Application.ExportedReports;
using Avera.Domain.Application.OverlayImages;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;
using System.Windows.Input;

namespace Avera.Application.ML.Process
{
    internal sealed class ProcessCommandHandler(
        IApplicationDbContext dbContext,
        IMLService mLService,
        // Temporary Logger
        ILogger<ProcessCommandHandler> logger,
        IUserContext userContext,
        UserManager<User> userManager,
        IDateTimeProvider dateTime) : ICommandHandler<ProcessCommand, ProcessResponse>
    {
        public async Task<Result<ProcessResponse>> Handle(ProcessCommand command, CancellationToken cancellationToken)
        {
            if (userContext.TenantId == null)
                return Result.Failure<ProcessResponse>(TenantErrors.NotMember);

            var user = await userManager.FindByIdAsync(userContext.UserId.ToString());

            if (user!.IsSuspended)
                return Result.Failure<ProcessResponse>(UserErrors.IsSuspended);

            logger.LogInformation("Processing ML for Case with Id: {CaseId}", command.CaseId);
            var selectedCase = await dbContext.Cases
                                                    .Include(c => c.CaseImages)
                                                    .FirstOrDefaultAsync(c => c.Id == command.CaseId
                                                        , cancellationToken);

            logger.LogInformation("Selected Case with Id: {CaseId} has {NumberOfImages} images", selectedCase!.Id, selectedCase.CaseImages.Count);
            var processRequest = new ProcessRequest(
                CaseName: selectedCase!.CaseCode,
                QuestionedImageUrl: selectedCase.CaseImages
                                    .Where(ci => ci.Type == ImageType.Suspected)
                                    .Select(ci => ci.FileName)
                                    .FirstOrDefault()!,
                ReferenceImageUrls: selectedCase.CaseImages
                                    .Where(ci => ci.Type == ImageType.Reference)
                                    .Select(ci => ci.FileName)
                                    .ToList()
            );

            logger.LogInformation("Sending process request for Case with Id: {CaseId}", selectedCase!.Id);  
            ProcessMLResponse? response = await mLService.ProcessAsync(processRequest, cancellationToken);
            List<GradCamImageDto> gradCamImages = new List<GradCamImageDto>();
            
            foreach (var blobId in response.GradcamBlobId)
            {
                if (blobId.EndsWith("output.pdf", StringComparison.OrdinalIgnoreCase))
                {
                    await AddExportedFileToCaseAsync(dbContext, selectedCase!.Id, blobId, dateTime, cancellationToken);
                    continue;
                }
                
                var parsedGradcamBlob = ParsedGradcamBlob(blobId);

                var gradCamImage = new GradCamImage
                {
                    Id = Guid.NewGuid(),
                    CaseId = selectedCase!.Id,
                    Slot = parsedGradcamBlob.Slot,
                    Type = parsedGradcamBlob.Type,
                    BlobPath = parsedGradcamBlob.BlobPath
                };
                
                string slot = parsedGradcamBlob.Slot.ToString();
                string variant = parsedGradcamBlob.Type.ToString();

                var GradCamImageDto = new GradCamImageDto(
                    Slot: slot,
                    Variant: variant,
                    ImageId: gradCamImage.Id
                );
                gradCamImages.Add(GradCamImageDto);
                await dbContext.GradCamImages.AddAsync(gradCamImage, cancellationToken);
            }

                var processReponse = new ProcessResponse(
                CaseName: response.CaseName,
                ConfidenceForged: response.ConfidenceForged,
                ConfidenceGenuine: response.ConfidenceGenuine,
                GradcamImages: gradCamImages,
                Distance: response.Distance,
                Threshold: response.Threshold,
                Verdict: response.Verdict
            );

            logger.LogInformation("Received process response for Case with Id: {CaseId} with status: {Status}", selectedCase!.Id, response);

            
            return processReponse;
        }

        private static ParsedGradcamBlob ParsedGradcamBlob(string blobPath)
        {
            var parts = blobPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            var folder = parts[1];
            var fileName = Path.GetFileNameWithoutExtension(parts[2]);

            GradCamSlot slot = folder switch
            {
                "genuine_1" => GradCamSlot.Reference1,
                "genuine_2" => GradCamSlot.Reference2,
                "genuine_3" => GradCamSlot.Reference3,
                "genuine_4" => GradCamSlot.Reference4,
                "suspected" => GradCamSlot.Suspected,
                _ => throw new ArgumentException($"Invalid folder name: {folder}")
            };
    
            GradCamVariant variant = fileName.EndsWith("_original", StringComparison.OrdinalIgnoreCase) ? GradCamVariant.Original:
                                     fileName.EndsWith("_heatmap", StringComparison.OrdinalIgnoreCase) ? GradCamVariant.Heatmap:
                                     fileName.EndsWith("_overlay", StringComparison.OrdinalIgnoreCase) ? GradCamVariant.Overlay:
                                     fileName.EndsWith("_bbox", StringComparison.OrdinalIgnoreCase) ? GradCamVariant.BoundingBox:
                                     fileName.EndsWith("_stroke_diff", StringComparison.OrdinalIgnoreCase) ? GradCamVariant.StrokeDiff:
                                     throw new ArgumentException($"Invalid file name: {fileName}");

            return new ParsedGradcamBlob(slot, variant, blobPath);

        }

        private static async Task AddExportedFileToCaseAsync(IApplicationDbContext dbContext, Guid caseId, string blobPath, IDateTimeProvider dateTime, CancellationToken cancellationToken)
        {
            var exportedFile = new ExportedReport
            {
                Id = Guid.NewGuid(),
                CaseId = caseId,
                BlobPath = blobPath,
                CreatedAt = dateTime.PhilippineNow

            };

            await dbContext.ExportedReports.AddAsync(exportedFile, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}