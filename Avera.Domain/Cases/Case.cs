using Avera.Domain.Application.CaseImages;
using Avera.Domain.Application.ExportedReports;
using Avera.Domain.Application.OverlayImages;
using Avera.Domain.Cases;
using SharedKernel;


namespace Avera.Domain.Application.Cases
{
    public sealed class Case : Entity
    {
        public Guid Id { get; set; }
        public string CaseCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = "No Subject";
        public AnalysisType AnalysisType { get; set; }
        public Priority Priority { get; set; }
        public string Notes { get; set; } = string.Empty;
        public Status Status { get; set; }
        public DateTime? DeletedAt { get; set; } = DateTime.MaxValue;
        public DateTime CreatedAt { get; set; }
        public string? MLVerdict { get; set; }
        public Guid? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewNote { get; set; }
        public FinalVerdict FinalVerdict { get; set; } = FinalVerdict.None;
        public bool IsPdfExportAllowed { get; set; } = false;
        public MLResponse? MLResponse { get; set; }


        // Navigation Properties
        public Guid CreatedByUserId { get; set; }
        public Guid? TenantId { get; set; }
        public List<CaseImage> CaseImages { get; set; } = new();
        public List<ExportedReport> ExportedReports { get; set; } = new();
        public List<GradCamImage> GradCamImages { get; set; } = new();

        // Static Properties
        public static string OutputBlob => $"output.pdf";

        public void Review(
            Guid reviewerId,
            FinalVerdict finalVerdict,
            string? reviewNote,
            bool isPdfExportAllowed,
            DateTime reviewedAt)
        {
            ReviewedBy = reviewerId;
            ReviewedAt = reviewedAt;
            ReviewNote = reviewNote;
            FinalVerdict = finalVerdict;
            IsPdfExportAllowed = isPdfExportAllowed;
            Status = Status.Reviewed;
        }
    }
}
