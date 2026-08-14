using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Identity.Tenants;
using SharedKernel;

namespace Avera.Application.Tenants.GetMemberById
{
    public sealed class GetMemberByIdCommandHandler
    (
        IAdminService adminService
    ) : ICommandHandler<GetMemberByIdCommand, TenantMemberDto>
    {
        public async Task<Result<TenantMemberDto>> Handle(GetMemberByIdCommand command, CancellationToken cancellationToken)
        {
            return await adminService.GetMemberByIdAsync(command.UserId, cancellationToken);
        }
    }
}