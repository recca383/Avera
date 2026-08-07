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
        public async Task<Result<GetCasesQueryResult>> Handle(GetCasesQuery query, CancellationToken cancellationToken)
        {
            IQueryable<Case>? cases = applicationDbContext.Cases.AsQueryable();

            if (userContext.TenantId == null)
                return Result.Failure<GetCasesQueryResult>(TenantErrors.NotMember);

            // Tenant Filtration
            cases = cases.Where(
                c => c.TenantId == userContext.TenantId
            );

            if (query.CaseStatus.HasValue)
                cases = cases.Where(c => c.Status == query.CaseStatus.Value);

            if (query.AnalysisPriority.HasValue)
                cases = cases.Where(c => c.Priority == query.AnalysisPriority.Value);

            if (query.AnalysisType.HasValue)
                cases = cases.Where(c => c.AnalysisType == query.AnalysisType.Value);

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

                if(createdByUser == null)
                    return Result.Failure<GetCasesQueryResult>(UserErrors.UserNotFound);

                var createdByUser_FullName = createdByUser.FirstName + " " + createdByUser.LastName;

                var selectedCase = new CaseDto(
                    pagedCase.Id,
                    pagedCase.CaseCode,
                    pagedCase.SubjectName,
                    createdByUser_FullName,
                    pagedCase.Priority,
                    pagedCase.CreatedAt,
                    pagedCase.Status,
                    pagedCase.AnalysisType,
                    IS_CASE_DELETED
                );
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