using SharedKernel;

namespace Avera.Domain.Identity.MemberRequests.Events
{
    public sealed record MemberRequestRejectedDomainEvent(
        Guid MemberRequestId,
        Guid TenantId,
        Guid UserId,
        Guid ReviewedByUserId,
        DateTime ReviewedAt
    ) : IDomainEvent;
}
