using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Tenants.Rename
{
    internal sealed class RenameTenantCommandHandler(IAdminService adminService) : ICommandHandler<RenameTenantCommand>
    {
        public async Task<Result> Handle(RenameTenantCommand command, CancellationToken cancellationToken)
        {
            return await adminService.RenameOrganization(command.newName, cancellationToken);
        }
    }
}
