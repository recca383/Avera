using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Application.Cases;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Avera.Application.Cases.GetById
{
    internal sealed class GetCaseByIdQueryHandler (
        IApplicationDbContext dbContext,
        IUserContext userContext,
        UserManager<User> userManager) : IQueryHandler<GetCaseByIdQuery, GetCaseByIdQueryResult>
    {
        public async Task<Result<GetCaseByIdQueryResult>> Handle(GetCaseByIdQuery query, CancellationToken cancellationToken)
        {
            if (userContext.TenantId == null)
                return Result.Failure<GetCaseByIdQueryResult>(TenantErrors.NotMember);

            var user = await userManager.FindByIdAsync(userContext.UserId.ToString());

            if (user!.IsSuspended)
                return Result.Failure<GetCaseByIdQueryResult>(UserErrors.IsSuspended);

            var queryResult = await dbContext.Cases
                        .Where(c => 
                            c.Id == query.CaseId)
                        .Include(c => c.GradCamImages)
                .FirstOrDefaultAsync(cancellationToken);

            if(queryResult == null)
                return Result.Failure<GetCaseByIdQueryResult>(CaseErrors.CaseNotFound);

            var createdByUser = await userManager.FindByIdAsync(queryResult.CreatedByUserId.ToString());

            if(createdByUser == null)
                return Result.Failure<GetCaseByIdQueryResult>(UserErrors.UserNotFound);

            var createdByUser_FullName = createdByUser.FirstName + " " + createdByUser.LastName;


            MLResponseDto mlResponseDto = null;

            var gradCamResults = queryResult.GradCamImages.Select(g => new GradCamDto(g.Slot, g.Type, g.Id)).ToList();

            if(queryResult.MLResponse != null)
            {
                mlResponseDto = new MLResponseDto(
                queryResult.MLResponse.ConfidenceForged,
                queryResult.MLResponse.ConfidenceGenuine,
                queryResult.MLResponse.Distance,
                gradCamResults,
                queryResult.MLResponse.Threshold,
                queryResult.MLResponse.Verdict
                );
            }

            var IsCaseViewed = await dbContext.CaseViews.AnyAsync(c => c.CaseId == queryResult.Id, cancellationToken);

            var finalCase = new CaseDto(
                queryResult.Id,
                queryResult.CaseCode,
                queryResult.SubjectName,
                createdByUser_FullName,
                queryResult.Priority,
                queryResult.CreatedAt,
                queryResult.Status,
                queryResult.DocumentType,
                queryResult.OptionalDocumentType!,
                queryResult.DeletedAt.HasValue,
                mlResponseDto,
                queryResult.ReviewedBy,
                queryResult.ReviewedAt,
                queryResult.ReviewNote,
                queryResult.FinalVerdict,
                queryResult.IsPdfExportAllowed,
                IsCaseViewed
            );

            return Result.Success(new GetCaseByIdQueryResult(finalCase!));
        }
    }
}
