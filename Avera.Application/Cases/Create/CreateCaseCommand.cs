using Avera.Application.Messaging;
using Avera.Domain.Application.Cases;

namespace Avera.Application.Cases.Create
{
    public class CreateCaseCommand : ICommand
    {
        public string SubjectName { get; set; } = "No Subject";
        public Guid ExaminerId { get; set; }
        public AnalysisType AnalysisType { get; set; }
        public Priority Priority { get; set; }
    }
}