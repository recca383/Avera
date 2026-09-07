using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.MemberRequests.GetPending
{
    public sealed record GetPendingQueryResponse(
        Guid RequestId,
        string Name,
        DateTime RequestedAt
        );
}
