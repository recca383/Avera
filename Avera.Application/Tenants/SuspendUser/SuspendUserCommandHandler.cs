using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Tenants.SuspendUser
{
    internal sealed class SuspendUserCommandHandler(IAdminService adminService) : ICommandHandler<SuspendUserCommand>
    {
        public async Task<Result> Handle(SuspendUserCommand command, CancellationToken cancellationToken)
        {
            return await adminService.SuspendUserAsync(command.UserId, cancellationToken);
        }
    }
}
