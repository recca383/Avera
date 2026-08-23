using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using SharedKernel;

namespace Avera.Application.Tenants.Create
{
    internal sealed class CreateTenantCommandHandler
    (
        IAdminService adminService
    ) : ICommandHandler<CreateTenantCommand, string>
    {
        public async Task<Result<string>> Handle(CreateTenantCommand command, CancellationToken cancellationToken)
        {
            return await adminService.CreateTenantAsync(command.Name, cancellationToken);
        }
    }
}