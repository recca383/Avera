using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.Authentication.ResetPassword
{
    public sealed record ResetPasswordCommand(
        string Token,
        string Password
    )
    : ICommand;
}