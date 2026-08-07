using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Application.Cases;
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
        private const bool IS_CASE_DELETED = false;
        public async Task<Result<GetCaseByIdQueryResult>> Handle(GetCaseByIdQuery query, CancellationToken cancellationToken)
        {
            var queryResult = await dbContext.Cases
                        .Where(c => 
                            c.Id == query.CaseId &&
                            c.TenantId == userContext.TenantId)
                .FirstOrDefaultAsync(cancellationToken);

            //             .Select(c => new GetCaseByIdQueryResult(new CaseDto
            // (
            //     c.Id,
            //     c.CaseCode,
            //     c.SubjectName,
            //     "Unknown",
            //     c.Priority,
            //     c.CreatedAt,
            //     c.Status,
            //     c.AnalysisType,
            //     false
            // )))

            if(queryResult == null)
                return Result.Failure<GetCaseByIdQueryResult>(CaseErrors.CaseNotFound);

            var createdByUser = await userManager.FindByIdAsync(queryResult.CreatedByUserId.ToString());

            if(createdByUser == null)
                return Result.Failure<GetCaseByIdQueryResult>(UserErrors.UserNotFound);

            var createdByUser_FullName = createdByUser.FirstName + " " + createdByUser.LastName;
            
            var finalCase = new CaseDto(
                queryResult.Id,
                queryResult.CaseCode,
                queryResult.SubjectName,
                createdByUser_FullName,
                queryResult.Priority,
                queryResult.CreatedAt,
                queryResult.Status,
                queryResult.AnalysisType,
                IS_CASE_DELETED
            );

            return Result.Success(new GetCaseByIdQueryResult(finalCase!));
        }
    }
}
