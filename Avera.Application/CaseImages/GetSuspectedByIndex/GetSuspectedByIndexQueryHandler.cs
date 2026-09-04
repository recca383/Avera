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

namespace Avera.Application.CaseImages.GetSuspectedByIndex
{
    internal sealed class GetSuspectedByIndexQueryHandler(
        IApplicationDbContext applicationDbContext,
        IBlobStorageService blobStorageService,
        // Temporary logger
        ILogger<GetSuspectedByIndexQueryHandler> logger,
        IUserContext userContext,
        UserManager<User> userManager
    ) : IQueryHandler<GetSuspectedByIndexQuery, GetSuspectedByIndexQueryResponse>
    {
        public async Task<Result<GetSuspectedByIndexQueryResponse>> Handle(GetSuspectedByIndexQuery query, CancellationToken cancellationToken)
        {
            if (userContext.TenantId == null)
                return Result.Failure<GetSuspectedByIndexQueryResponse>(TenantErrors.NotMember);

            var user = await userManager.FindByIdAsync(userContext.UserId.ToString());

            if (user!.IsSuspended)
                return Result.Failure<GetSuspectedByIndexQueryResponse>(UserErrors.IsSuspended);

            var selectedCase = await applicationDbContext.Cases.Include(c => c.CaseImages)
            .FirstOrDefaultAsync(c => c.Id == query.CaseId, cancellationToken);

            if (selectedCase is null)
            {
                logger.LogWarning("Case with id {CaseId} not found", query.CaseId);
                return Result.Failure<GetSuspectedByIndexQueryResponse>(CaseErrors.CaseNotFound);
            }
                
            var caseImage = selectedCase?.CaseImages
                    .FirstOrDefault(ci => ci.Type == ImageType.Suspected && ci.Index == query.Index);

            
            logger.LogInformation("Case has {CaseImageCount} suspected images", selectedCase?.CaseImages.Count(ci => ci.Type == ImageType.Suspected));
            if (caseImage is null)
            {
                logger.LogWarning("Case image with index {Index} not found", query.Index);
                return Result.Failure<GetSuspectedByIndexQueryResponse>(CaseImageErrors.CaseImageNotFound);
            }

            logger.LogInformation("Case Image Url: {CaseImageUrl}", caseImage.BlobName);
            var reference = await blobStorageService.DownloadAsync(caseImage.BlobName, cancellationToken);

            var response = new GetSuspectedByIndexQueryResponse(
                selectedCase!.Id,
                reference!,
                caseImage.MimeType
                );
                
            return Result.Success(response);
        }
    }
}