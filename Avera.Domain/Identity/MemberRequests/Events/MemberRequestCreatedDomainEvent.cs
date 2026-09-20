using SharedKernel;

namespace Avera.Domain.Identity.MemberRequests.Events
{
    public sealed record MemberRequestCreatedDomainEvent(
        Guid MemberRequestId,
        Guid TenantId,
        Guid UserId,
        DateTime CreatedAt
    ) : IDomainEvent;
}
