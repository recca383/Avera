using Avera.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Authentication.VerifyEmail
{
    public sealed record VerifyEmailCommand(
        Guid UserId,
        string Token,
        string? Type,
        string? Email
        ) : ICommand;
}
