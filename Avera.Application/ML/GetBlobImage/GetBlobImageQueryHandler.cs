using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Abstractions.Storage;
using Avera.Domain.Application.CaseImages;
using Avera.Domain.Application.Cases;
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

            // Reject anything that isn't a known folder or contains path traversal characters
            if (!AllowedFolders.Contains(query.Folder) ||
                query.FileName.Contains("..") ||
                query.FileName.Contains('/') ||
                query.FileName.Contains('\\'))
            {
                logger.LogWarning("Invalid blob image request for Case {CaseId}: {Folder}/{FileName}", query.CaseId, query.Folder, query.FileName);
                return Result.Failure<GetBlobImageResponse>(CaseImageErrors.CaseImageNotFound);
            }

            // Backend reconstructs the full blob path — frontend never sees or sends it
            var blobPath = $"{selectedCase.CaseCode}/{query.Folder}/{query.FileName}";

            var stream = await blobStorageService.DownloadAsync(blobPath, cancellationToken);

            if (stream is null)
            {
                logger.LogWarning("Blob not found at path: {BlobPath}", blobPath);
                return Result.Failure<GetBlobImageResponse>(CaseImageErrors.CaseImageNotFound);
            }
            
            var contentType = query.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                ? "image/png"
                : "application/octet-stream";

            return Result.Success(new GetBlobImageResponse(stream, contentType));
        }
    }
}