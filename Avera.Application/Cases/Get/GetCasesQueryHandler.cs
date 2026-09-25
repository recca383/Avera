using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.CaseImages;
using Avera.Application.Cases.Get;
using Avera.Domain.Application.Cases;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SharedKernel;

namespace Avera.Application.Cases.Get
{
    internal class GetCaseQueryHandler(
        IApplicationDbContext applicationDbContext,
        IUserContext userContext,
        UserManager<User> userManager) 
        : IQueryHandler<GetCasesQuery, GetCasesQueryResult>
    {
        private static readonly ILogger logger = Log.ForContext<GetCaseQueryHandler>();
        // noop to trigger rebuild
        public async Task<Result<GetCasesQueryResult>> Handle(GetCasesQuery query, CancellationToken cancellationToken)
        {
            if (userContext.TenantId == null)
                return Result.Failure<GetCasesQueryResult>(TenantErrors.NotMember);

            var user = await userManager.FindByIdAsync(userContext.UserId.ToString());

            if (user!.IsSuspended)
                return Result.Failure<GetCasesQueryResult>(UserErrors.IsSuspended);

            IQueryable<Case>? cases = applicationDbContext.Cases.Include(c => c.CaseViews).AsQueryable();

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

            foreach (var pagedCase in pagedCases)
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

                pagedCase.CaseViews ??= [];

                // determine if the current user has viewed this case result
                var IsViewedByCurrentUser = pagedCase
                                            .CaseViews
                                            .Any(u => u.UserId == userContext.UserId && u.CaseId == pagedCase.Id);

                var finalCase = new CaseDto(
                    pagedCase.Id,
                    pagedCase.CaseCode,
                    pagedCase.SubjectName,
                    createdByUser_FullName,
                    createdByUser.Id,
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
                    pagedCase.IsFlaggedForInternalReview,
                    IsViewedByCurrentUser,
                    pagedCase.TimeTakenForAnalysis
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