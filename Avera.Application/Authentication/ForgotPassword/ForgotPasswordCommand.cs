using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.Common;

namespace Avera.Application.Authentication.ForgotPassword
{
    public sealed record ForgotPasswordCommand(
        string Email
    ) : ICommand<TokenExpiryResponse>;
}