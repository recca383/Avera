using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.Authentication.Login
{
    public sealed record LoginCommand(
        string Email,
        string Password
    )
    : ICommand<LoginResponse>;

}