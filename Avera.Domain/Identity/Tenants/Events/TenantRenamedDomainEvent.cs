namespace Avera.Domain.Identity.Tenants.Events;

public sealed class TenantRenamedDomainEvent : SharedKernel.IDomainEvent
{
    public TenantRenamedDomainEvent(Guid tenantId, string oldName, string newName, Guid renamedBy, DateTime renamedAt)
    {
        TenantId = tenantId;
        OldName = oldName;
        NewName = newName;
        RenamedBy = renamedBy;
        RenamedAt = renamedAt;
    }

    public Guid TenantId { get; }
    public string OldName { get; }
    public string NewName { get; }
    public Guid RenamedBy { get; }
    public DateTime RenamedAt { get; }
}
