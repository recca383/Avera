using SharedKernel;

namespace Avera.Domain.Identity.Users.Events
{
    public sealed record UserSuspendedDomainEvent(
        Guid UserId,
        Guid? TenantId,
        DateTime SuspendedAt
    ) : IDomainEvent;
}
