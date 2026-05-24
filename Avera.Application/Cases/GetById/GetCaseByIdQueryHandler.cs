using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Application.Cases;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Avera.Application.Cases.GetById
{
    internal sealed class GetCaseByIdQueryHandler (IApplicationDbContext dbContext) : IQueryHandler<GetCaseByIdQuery, GetCaseByIdQueryResult>
    {
        public async Task<Result<GetCaseByIdQueryResult>> Handle(GetCaseByIdQuery query, CancellationToken cancellationToken)
        {
            var queryResult = await dbContext.Cases
                        .Where(c => c.Id == query.CaseId)
                        .Select(c => new GetCaseByIdQueryResult(new CaseDto
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
                        )))
                .FirstOrDefaultAsync(cancellationToken);
                
            return Result.Success(queryResult!);
        }
    }
}
