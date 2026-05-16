using Avera.Application.CaseImages;
using Avera.Domain.Application.Cases;

namespace Avera.Application.Cases
{
    public sealed class CaseDto
    {
        public Guid Id { get; set; }
        public string CaseCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = "No Subject";
        public string Examiner { get; set; } = string.Empty;
        public Priority Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public Status CaseStatus { get; set; }
        public AnalysisType AnalysisType { get; set; }
        public bool IsDeleted { get; set; }
    }

}