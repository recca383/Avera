using Avera.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Authentication.ResendVerificationEmail
{
    public sealed record ResendVerificationEmailCommand(string Email, string? Type, string? NewEmail) : ICommand;
}
