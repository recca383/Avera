using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.Delete
{
    public sealed record DeleteCaseCommand (Guid CaseId): ICommand;
}