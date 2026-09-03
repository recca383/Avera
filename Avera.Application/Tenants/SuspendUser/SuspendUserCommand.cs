using Avera.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Tenants.SuspendUser
{
    public sealed record SuspendUserCommand(Guid UserId) : ICommand;
}
