using Avera.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.MemberRequests.GetPending
{
    public sealed record GetPendingQuery() : IQuery<List<GetPendingQueryResponse>>;
}
