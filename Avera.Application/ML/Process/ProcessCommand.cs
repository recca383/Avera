using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.ML.Process
{
    public sealed record ProcessCommand(Guid CaseId) : ICommand<ProcessResponse>;
}