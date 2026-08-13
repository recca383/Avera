using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.Tenants.Create 
{
    public sealed record CreateTenantCommand(
        string Name
    ) : ICommand<Guid>;
}