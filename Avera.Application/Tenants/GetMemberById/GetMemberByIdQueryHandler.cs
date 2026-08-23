using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Identity.Tenants;
using SharedKernel;

namespace Avera.Application.Tenants.GetMemberById
{
    internal sealed class GetMemberByIdQueryHandler
    (
        IAdminService adminService
    ) : IQueryHandler<GetMemberByIdQuery, TenantMemberDto>
    {
        public async Task<Result<TenantMemberDto>> Handle(GetMemberByIdQuery command, CancellationToken cancellationToken)
        {
            return await adminService.GetMemberByIdAsync(command.UserId, cancellationToken);
        }
    }
}