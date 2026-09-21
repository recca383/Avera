using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.Tenants.GetMemberCaseHistory;

public sealed record GetMemberCaseHistoryQuery(Guid UserId)
    : IQuery<List<MemberCaseHistoryItem>>;
