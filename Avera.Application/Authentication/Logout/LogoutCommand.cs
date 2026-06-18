using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.Authentication.Logout
{
    public sealed record LogoutCommand(Guid UserId) : ICommand;
}