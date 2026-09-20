using Microsoft.AspNetCore.SignalR;

namespace Avera.Application.Abstractions.NotificationHub;

public interface INotificationHubNotifier
{
    IHubClients Clients { get; }
}
