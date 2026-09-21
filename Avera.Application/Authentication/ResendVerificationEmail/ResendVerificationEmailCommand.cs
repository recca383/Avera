using Avera.Application.Abstractions.Messaging;
using Avera.Application.Authentication.Common;

namespace Avera.Application.Authentication.ResendVerificationEmail;

public sealed record ResendVerificationEmailCommand(
    string Email,
    string? Type,
    string? NewEmail
) : ICommand<TokenExpiryResponse>;