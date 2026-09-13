using Avera.Domain.Cases;
using SharedKernel;

namespace Avera.Domain.Cases.Events
{
    public sealed record CaseReviewCompletedDomainEvent(
        Guid CaseId,
        Guid TenantId,
        Guid AnalystUserId,
        FinalVerdict FinalVerdict,
        DateTime ReviewedAt
        ) : IDomainEvent;
}
