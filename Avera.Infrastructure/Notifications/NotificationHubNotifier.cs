using Avera.Application.Abstractions.NotificationHub;
using Microsoft.AspNetCore.SignalR;

namespace Avera.Infrastructure.Notifications;

internal sealed class NotificationHubNotifier : INotificationHubNotifier
{
    private readonly IHubContext<Avera.Application.Hubs.NotificationHub> _hub;

    public NotificationHubNotifier(IHubContext<Avera.Application.Hubs.NotificationHub> hub)
    {
        _hub = hub;
    }

    public IHubClients Clients => _hub.Clients;
}
