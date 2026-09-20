namespace Avera.Application.Tenants.Unsuspend
{
    using Avera.Application.Abstractions.Messaging;

    public sealed record UnsuspendUserCommand(Guid UserId) : ICommand;
}
