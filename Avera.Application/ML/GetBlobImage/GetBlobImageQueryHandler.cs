using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Abstractions.Storage;
using Avera.Domain.Application.CaseImages;
using Avera.Domain.Application.Cases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Avera.Application.ML.GetBlobImage
{
    internal sealed class GetBlobImageQueryHandler(
        IApplicationDbContext dbContext,
        IBlobStorageService blobStorageService,
        ILogger<GetBlobImageQueryHandler> logger
    ) : IQueryHandler<GetBlobImageQuery, GetBlobImageResponse>
    {
        private static readonly HashSet<string> AllowedFolders = new(StringComparer.OrdinalIgnoreCase)
        {
            "genuine_1", "genuine_2", "genuine_3", "genuine_4", "suspected"
        };

        public async Task<Result<GetBlobImageResponse>> Handle(GetBlobImageQuery query, CancellationToken cancellationToken)
        {
            var selectedCase = dbContext.Cases.FirstOrDefault(c => c.Id == query.CaseId);

            if (selectedCase is null)
            {
                logger.LogWarning("Case with Id: {CaseId} not found", query.CaseId);
                return Result.Failure<GetBlobImageResponse>(CaseErrors.CaseNotFound);
            }

            var image = await dbContext.GradCamImages.FirstOrDefaultAsync(
                ci => ci.CaseId == query.CaseId 
                && ci.Id == query.ImageId, cancellationToken);

            
            var imageBlobPath = image?.BlobPath;

            var stream = await blobStorageService.DownloadAsync(imageBlobPath!, cancellationToken);

            if (stream is null)
            {
                logger.LogWarning("Blob not found at path: {BlobPath}", imageBlobPath);
                return Result.Failure<GetBlobImageResponse>(CaseImageErrors.CaseImageNotFound);
            }
            
            var contentType = imageBlobPath!.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                ? "image/png"
                : "application/octet-stream";

            return Result.Success(new GetBlobImageResponse(stream, contentType));
        }
    }
}