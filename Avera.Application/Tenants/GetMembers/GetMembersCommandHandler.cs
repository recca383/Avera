using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Identity.Tenants;
using SharedKernel;

namespace Avera.Application.Tenants.GetMembers
{
    internal sealed class GetMembersCommandHandler(
        IAdminService adminService
    ) : ICommandHandler<GetMembersCommand, List<TenantMemberDto>>
    {
        public async Task<Result<List<TenantMemberDto>>> Handle(GetMembersCommand command, CancellationToken cancellationToken)
        {
            return await adminService.GetMembersAsync(cancellationToken);
        }
    }
}