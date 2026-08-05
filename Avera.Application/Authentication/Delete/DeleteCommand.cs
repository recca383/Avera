using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.Authentication.Delete
{
    public sealed record DeleteCommand(CancellationToken CancellationToken) : ICommand;
}