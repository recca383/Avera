using Avera.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Tenants.SetMemberCountLimit
{
    public sealed record SetMemberCountLimitCommand(int NewLimit) : ICommand;
}
