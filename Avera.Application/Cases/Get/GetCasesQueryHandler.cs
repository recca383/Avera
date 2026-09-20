using Avera.Application.Cases.Get;
using SharedKernel;
using Avera.Application.Abstractions.Databases;
using Avera.Domain.Application.Cases;
using Avera.Application.CaseImages;
using Microsoft.EntityFrameworkCore;
using Avera.Application.Abstractions.Messaging;
using Serilog;
using Avera.Application.Abstractions.Authentication;
using Avera.Domain.Identity.Users;
using Avera.Domain.Identity.Tenants;
using Microsoft.AspNetCore.Identity;

namespace Avera.Application.Cases.Get
{
    internal class GetCaseQueryHandler(
        IApplicationDbContext applicationDbContext,
        IUserContext userContext,
        UserManager<User> userManager) 
        : IQueryHandler<GetCasesQuery, GetCasesQueryResult>
    {
        private static readonly ILogger logger = Log.ForContext<GetCaseQueryHandler>();
        private const bool IS_CASE_DELETED = false;
        // noop to trigger rebuild
        public async Task<Result<GetCasesQueryResult>> Handle(GetCasesQuery query, CancellationToken cancellationToken)
        {
            if (userContext.TenantId == null)
                return Result.Failure<GetCasesQueryResult>(TenantErrors.NotMember);

            var user = await userManager.FindByIdAsync(userContext.UserId.ToString());

            if (user!.IsSuspended)
                return Result.Failure<GetCasesQueryResult>(UserErrors.IsSuspended);

            IQueryable<Case>? cases = applicationDbContext.Cases.AsQueryable();

            if (userContext.TenantId == null)
                return Result.Failure<GetCasesQueryResult>(TenantErrors.NotMember);

            if (query.CaseStatus.HasValue)
                cases = cases.Where(c => c.Status == query.CaseStatus.Value);

            if (query.AnalysisPriority.HasValue)
                cases = cases.Where(c => c.Priority == query.AnalysisPriority.Value);

            if (query.AnalysisType.HasValue)
                cases = cases.Where(c => c.DocumentType == query.AnalysisType.Value);

            int totalCount = cases.Count();

            int page = query.Page ?? 1;
            int pageSize = query.PageSize ?? 10;
            
            
            var pagedCases = cases
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            List<CaseDto> pagedCasesList = new();

            foreach(var pagedCase in pagedCases)
            {
                var createdByUser = await userManager.FindByIdAsync(pagedCase.CreatedByUserId.ToString());

                if (createdByUser == null)
                    continue;

                var createdByUser_FullName = createdByUser.FirstName + " " + createdByUser.LastName;

                var gradCamResults = pagedCase.GradCamImages.Select(g => new GradCamDto(g.Slot, g.Type, g.Id)).ToList();

                MLResponseDto mlResponseDto = null;

                if (pagedCase.MLResponse != null)
                {
                    mlResponseDto = new MLResponseDto(
                    pagedCase.MLResponse.ConfidenceForged,
                    pagedCase.MLResponse.ConfidenceGenuine,
                    pagedCase.MLResponse.Distance,
                    gradCamResults,
                    pagedCase.MLResponse.Threshold,
                    pagedCase.MLResponse.Verdict
                    );
                }

                // determine if the current user has viewed this case result
                var hasViewed = await applicationDbContext.CaseViews.AnyAsync(cv => cv.CaseId == pagedCase.Id && cv.UserId == userContext.UserId, cancellationToken);

                var finalCase = new CaseDto(
                    pagedCase.Id,
                    pagedCase.CaseCode,
                    pagedCase.SubjectName,
                    createdByUser_FullName,
                    pagedCase.Priority,
                    pagedCase.CreatedAt,
                    pagedCase.Status,
                    pagedCase.DocumentType,
                    pagedCase.OptionalDocumentType!,
                    pagedCase.DeletedAt.HasValue,
                    mlResponseDto,
                    pagedCase.ReviewedBy,
                    pagedCase.ReviewedAt,
                    pagedCase.ReviewNote,
                    pagedCase.FinalVerdict,
                    pagedCase.IsPdfExportAllowed,
                    hasViewed
                );

                pagedCasesList.Add(finalCase);
            }

            GetCasesQueryResult? result = new(
                 Cases: pagedCasesList,
                 TotalCount: totalCount,
                 Page: page,
                 PageSize: pageSize
            );

            return Result.Success(result);
        }

    }
}