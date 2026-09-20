using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.Cases.View;

public sealed record MarkCaseViewedCommand(Guid CaseId) : ICommand;
