using SharedKernel;

namespace Avera.Domain.Identity.Users.Events
{
    public sealed record UserDeletedDomainEvent(
        Guid UserId,
        Guid? TenantId,
        DateTime DeletedAt
    ) : IDomainEvent;
}
