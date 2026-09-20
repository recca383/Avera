using SharedKernel;

namespace Avera.Domain.Cases.Events
{
    public sealed record CaseResultCreatedDomainEvent(
        Guid CaseId,
        Guid TenantId,
        Guid CreatedByUserId,
        DateTime CreatedAt
    ) : IDomainEvent;
}
