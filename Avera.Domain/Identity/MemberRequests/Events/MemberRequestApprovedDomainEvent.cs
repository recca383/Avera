using SharedKernel;

namespace Avera.Domain.Identity.MemberRequests.Events
{
    public sealed record MemberRequestApprovedDomainEvent(
        Guid MemberRequestId,
        Guid TenantId,
        Guid UserId,
        Guid ReviewedByUserId,
        DateTime ReviewedAt
    ) : IDomainEvent;
}
