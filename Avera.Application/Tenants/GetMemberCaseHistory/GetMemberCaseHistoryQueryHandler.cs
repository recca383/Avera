using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Avera.Application.Tenants.GetMemberCaseHistory;

internal sealed class GetMemberCaseHistoryQueryHandler(
    IApplicationDbContext applicationDbContext,
    IUserContext userContext,
    UserManager<User> userManager)
    : IQueryHandler<GetMemberCaseHistoryQuery, List<MemberCaseHistoryItem>>
{
    public async Task<Result<List<MemberCaseHistoryItem>>> Handle(
        GetMemberCaseHistoryQuery query,
        CancellationToken cancellationToken)
    {
        var tenantId = userContext.TenantId;
        if (!tenantId.HasValue)
            return Result.Failure<List<MemberCaseHistoryItem>>(TenantErrors.NotMember);

        var memberBelongsToTenant = await userManager.Users.AnyAsync(
            user => user.Id == query.UserId && user.TenantId == tenantId.Value,
            cancellationToken);

        if (!memberBelongsToTenant)
            return Result.Failure<List<MemberCaseHistoryItem>>(UserErrors.UserNotFound);

        var cases = await applicationDbContext.Cases
            .Where(caseItem =>
                caseItem.TenantId == tenantId.Value &&
                caseItem.CreatedByUserId == query.UserId)
            .OrderByDescending(caseItem => caseItem.CreatedAt)
            .Select(caseItem => new MemberCaseHistoryItem(
                caseItem.Id,
                caseItem.CaseCode,
                caseItem.SubjectName,
                caseItem.CreatedAt,
                caseItem.Status,
                caseItem.DocumentType,
                caseItem.OptionalDocumentType,
                caseItem.DeletedAt != DateTime.MaxValue))
            .ToListAsync(cancellationToken);

        return Result.Success(cases);
    }
}
