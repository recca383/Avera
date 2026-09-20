namespace Avera.Domain.Identity.Users.Events;

public sealed record UserUnsuspendedDomainEvent(
    Guid UserId,
    Guid? TenantId,
    DateTime UnsuspendedAt
): SharedKernel.IDomainEvent;
