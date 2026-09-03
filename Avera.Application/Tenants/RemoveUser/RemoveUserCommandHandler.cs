using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Tenants.RemoveFromTenant
{
    internal sealed class RemoveUserCommandHandler(
        IAdminService adminService) : ICommandHandler<RemoveUserCommand>
    {
        public async Task<Result> Handle(RemoveUserCommand command, CancellationToken cancellationToken)
        {
            return await adminService.RemoveUserAsync(command.userId, cancellationToken);
        }
    }
}
