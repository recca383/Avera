using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Tenants.SetMemberCountLimit
{
    internal sealed class SetMemberCountLimitCommandHandler(IAdminService adminService) : ICommandHandler<SetMemberCountLimitCommand>
    {
        public async Task<Result> Handle(SetMemberCountLimitCommand command, CancellationToken cancellationToken)
        {
            return await adminService.SetMemberCountLimit(command.NewLimit, cancellationToken);
        }
    }
}
