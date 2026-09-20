using System;
using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.Cases.Flag
{
    public sealed record ToggleCaseFlagCommand(Guid CaseId, bool IsFlagged) : ICommand;
}
