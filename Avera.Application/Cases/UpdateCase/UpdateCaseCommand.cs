using Avera.Application.Messaging;
using Avera.Domain.Application.Cases;

namespace Avera.Application.UpdateCase
{
    public sealed class UpdateCaseCommand : ICommand<Guid>
    {
        public Guid Id { get; set; }
        public string SubjectName { get; set; } = "No Subject";
        public Guid ExaminerId { get; set; }
        public AnalysisType AnalysisType { get; set; }
        public Priority Priority { get; set; }
    }
}