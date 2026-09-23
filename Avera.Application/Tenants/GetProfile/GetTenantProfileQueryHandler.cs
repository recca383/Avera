using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Avera.Application.Tenants.GetProfile
{
    internal sealed class GetTenantProfileQueryHandler(
        IIdentityDbContext db,
        IUserContext userContext
    ) : IQueryHandler<GetTenantProfileQuery, TenantProfileDto>
    {
        public async Task<Result<TenantProfileDto>> Handle(
            GetTenantProfileQuery query,
            CancellationToken cancellationToken)
        {
            var tenantId = userContext.TenantId;

            if (!tenantId.HasValue)
            {
                return Result.Failure<TenantProfileDto>(
                    TenantErrors.NotMember);
            }

            var profile = await db.Tenants
                .Where(t => t.Id == tenantId.Value)
                .Select(t => new TenantProfileDto(
                    t.Id,
                    t.Name,
                    t.InviteCode,
                    t.Users.Count,
                    t.CreatedAt,
                    t.MemberCountLimit))
                .FirstOrDefaultAsync(cancellationToken);

            if (profile is null)
            {
                return Result.Failure<TenantProfileDto>(
                    UserErrors.TenantNotFound);
            }

            return Result.Success(profile);
        }
    }
}