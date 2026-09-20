using Avera.Application.Abstractions.NotificationHub;
using Avera.Domain.Identity.Tenants.Events;

namespace Avera.Application.Tenants.Notifications;

internal sealed class TenantRenamedDomainEventHandler : SharedKernel.IDomainEventHandler<TenantRenamedDomainEvent>
{
    private readonly INotificationHubNotifier _notifier;

    public TenantRenamedDomainEventHandler(INotificationHubNotifier notifier)
    {
        _notifier = notifier;
    }

    public async Task Handle(TenantRenamedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        // Broadcast to all members in the tenant
        var payload = new
        {
            TenantId = domainEvent.TenantId,
            OldName = domainEvent.OldName,
            NewName = domainEvent.NewName,
            RenamedBy = domainEvent.RenamedBy,
            RenamedAt = domainEvent.RenamedAt
        };

        await _notifier.Clients.Group($"tenant:{domainEvent.TenantId}:members")
            .SendCoreAsync("TenantRenamed", new object[] { payload }, cancellationToken);
    }
}
