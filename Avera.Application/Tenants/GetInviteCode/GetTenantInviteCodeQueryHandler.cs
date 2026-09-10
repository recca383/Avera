using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Tenants.GetInviteCode;
using Microsoft.EntityFrameworkCore;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using SharedKernel;

internal sealed class GetTenantInviteCodeQueryHandler(
    IIdentityDbContext db,
    IUserContext userContext
) : IQueryHandler<GetTenantInviteCodeQuery, TenantInviteCodeDto>
{
    public async Task<Result<TenantInviteCodeDto>> Handle(
        GetTenantInviteCodeQuery query,
        CancellationToken cancellationToken)
    {
        var tenantId = userContext.TenantId;

        if (!tenantId.HasValue)
        {
            return Result.Failure<TenantInviteCodeDto>(
                TenantErrors.NotMember);
        }

        var inviteCode = await db.Tenants
            .Where(t => t.Id == tenantId.Value)
            .Select(t => t.InviteCode)
            .FirstOrDefaultAsync(cancellationToken);

        if (inviteCode is null)
        {
            return Result.Failure<TenantInviteCodeDto>(
                UserErrors.TenantNotFound);
        }

        return Result.Success(
            new TenantInviteCodeDto(inviteCode));
    }
}