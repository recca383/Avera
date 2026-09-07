using Avera.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.MemberRequests.Reject
{
    public sealed record RejectCommand(Guid MemberRequestId) : ICommand;
}
