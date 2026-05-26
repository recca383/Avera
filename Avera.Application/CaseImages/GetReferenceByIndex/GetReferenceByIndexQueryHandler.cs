using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Abstractions.Storage;
using Avera.Domain.Application.CaseImages;
using Avera.Domain.Application.Cases;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Avera.Application.CaseImages.GetReferenceByIndex
{
    internal sealed class GetReferenceByIndexQueryHandler(
        IApplicationDbContext applicationDbContext,
        IBlobStorageService blobStorageService,
        ILogger<GetReferenceByIndexQueryHandler> logger
    ) : IQueryHandler<GetReferenceByIndexQuery, GetReferenceByIndexQueryResponse>
    {
        public async Task<Result<GetReferenceByIndexQueryResponse>> Handle(GetReferenceByIndexQuery query, CancellationToken cancellationToken)
        {
            var selectedCase = await applicationDbContext.Cases.Include(c => c.CaseImages)
            .FirstOrDefaultAsync(c => c.Id == query.CaseId, cancellationToken);

            if (selectedCase is null)
            {
                logger.LogWarning("Case with id {CaseId} not found", query.CaseId);
                return Result.Failure<GetReferenceByIndexQueryResponse>(CaseErrors.CaseNotFound);
            }
                
            var caseImage = selectedCase?.CaseImages
                    .FirstOrDefault(ci => ci.Type == ImageType.Reference && ci.Index == query.Index);
                    
            logger.LogInformation("Case Image URL: {CaseImageUrl}", caseImage?.BlobName);
            logger.LogInformation("Case has {CaseImageCount} reference images", selectedCase?.CaseImages.Count(ci => ci.Type == ImageType.Reference));

            if (caseImage is null)
            {
                logger.LogWarning("Case image with index {Index} not found", query.Index);
                return Result.Failure<GetReferenceByIndexQueryResponse>(CaseImageErrors.CaseImageNotFound);
            }
            
            var reference = await blobStorageService.DownloadAsync(caseImage.BlobName, cancellationToken);

            var response = new GetReferenceByIndexQueryResponse(
                selectedCase!.Id,
                reference!,
                caseImage.MimeType
                );

            return Result.Success(response);
        }
    }
}