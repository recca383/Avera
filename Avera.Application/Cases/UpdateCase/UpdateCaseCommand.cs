using Avera.Application.Abstractions.Messaging;
using Avera.Domain.Application.Cases;

namespace Avera.Application.UpdateCase
{
    public sealed record UpdateCaseCommand(
        Guid Id,
        string SubjectName,
        AnalysisType AnalysisType,
        Priority Priority) : ICommand<Guid>;
}