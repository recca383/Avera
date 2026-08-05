using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.Authentication.ResetPassword
{
    public sealed record ResetPasswordCommand(
        string Email,
        string Token,
        string Password
    )
    : ICommand;
}