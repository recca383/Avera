using SharedKernel;

namespace Avera.Domain.Identity.Users.Events
{
    public sealed record UserRemovedFromTenantDomainEvent(
        Guid UserId,
        Guid? TenantId,
        Guid RemovedByUserId,
        DateTime RemovedAt
    ) : IDomainEvent;
}
