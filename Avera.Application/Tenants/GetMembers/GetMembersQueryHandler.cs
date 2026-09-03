using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Identity.Tenants;
using SharedKernel;

namespace Avera.Application.Tenants.GetMembers
{
    internal sealed class GetMembersQueryHandler(
        IAdminService adminService
    ) : IQueryHandler<GetMembersQuery, List<TenantMemberDto>>
    {
        public async Task<Result<List<TenantMemberDto>>> Handle(GetMembersQuery command, CancellationToken cancellationToken)
        {
            return await adminService.GetMembersAsync(cancellationToken);
        }
    }
}