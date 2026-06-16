using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.Authentication.ForgotPassword
{
    public sealed record ForgotPasswordCommand(
        string Email
    ) : ICommand;
}