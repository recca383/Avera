using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.Abstractions.Services;
using SharedKernel;

namespace Avera.Application.Tenants.Unsuspend;

internal sealed class UnsuspendUserCommandHandler(IAdminService adminService) : ICommandHandler<UnsuspendUserCommand>
{
    public async Task<Result> Handle(UnsuspendUserCommand command, CancellationToken cancellationToken)
    {
        return await adminService.UnsuspendUserAsync(command.UserId, cancellationToken);
    }
}
