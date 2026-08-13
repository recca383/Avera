using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using SharedKernel;

namespace Avera.Application.Tenants.Create
{
    internal sealed class CreateTenantCommandHandler
    (
        IAdminService adminService
    ) : ICommandHandler<CreateTenantCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(CreateTenantCommand command, CancellationToken cancellationToken)
        {
            return await adminService.CreateTenantAsync(command.Name, cancellationToken);
        }
    }
}