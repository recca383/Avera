using Avera.Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Application.Authentication.VerifyPasswordCode;

public sealed record VerifyPasswordCodeCommand(
    string Email,
    string Code
    ) : ICommand;
