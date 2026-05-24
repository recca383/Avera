using Avera.Application.Cases.Get;
using SharedKernel;
using Avera.Application.Abstractions.Databases;
using Avera.Domain.Application.Cases;
using Avera.Application.CaseImages;
using Microsoft.EntityFrameworkCore;
using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.Cases.Get
{
    internal class GetCaseQueryHandler(IApplicationDbContext applicationDbContext) : IQueryHandler<GetCasesQuery, GetCasesQueryResult>
    {
        public async Task<Result<GetCasesQueryResult>> Handle(GetCasesQuery query, CancellationToken cancellationToken)
        {
            IQueryable<Case>? cases = applicationDbContext.Cases.AsQueryable();

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
                .Take(pageSize)
                .Select(c => new CaseDto
                (
                    c.Id,
                    c.CaseCode,
                    c.SubjectName,
                    c.User != null ? c.User.UserName! : "Unknown",
                    c.Priority,
                    c.CreatedAt,
                    c.Status,
                    c.AnalysisType,
                    false
                ));

            List<CaseDto> pagedCasesList = await pagedCases.ToListAsync(cancellationToken);

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