using System.Windows.Input;
using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.ML.GetResults
{
    public record GetMLResultsCommand
    (
        Guid CaseId
    ) : ICommand<GetMLResultsResponse>;
}