using Avera.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.MemberRequests.Approve
{
    public sealed record ApproveCommand(Guid MemberRequestId) : ICommand;
}
