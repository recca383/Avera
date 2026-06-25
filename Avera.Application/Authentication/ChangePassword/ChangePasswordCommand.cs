using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.Authentication.ChangePassword
{
    public sealed record ChangePasswordCommand(
        string newPassword,
        string currentPassword
    ) : ICommand;
}