using SharedKernel;

namespace Avera.Domain.Cases.Events
{
    public sealed record CaseFlagToggledDomainEvent(
        Guid CaseId,
        Guid TenantId,
        Guid ToggledByUserId,
        bool IsFlagged,
        DateTime ToggledAt
    ) : IDomainEvent;
}
