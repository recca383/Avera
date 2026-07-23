using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avera.Domain.Application.CaseImages;
using Avera.Domain.Application.ExportedReports;
using Avera.Domain.Application.OverlayImages;
using Avera.Domain.Identity.Users;

namespace Avera.Domain.Application.Cases
{
    public sealed class Case
    {
        public Guid Id { get; set; }
        public string CaseCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = "No Subject";
        public AnalysisType AnalysisType { get; set; }
        public Priority Priority { get; set; }
        public string Notes { get; set; } = string.Empty;
        public Status Status { get; set; }
        public bool Verdict { get; set; }
        public DateTime DeletedAt { get; set; } = DateTime.MaxValue;
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public User? User { get; set; }
        public Guid UserId { get; set; }
        public List<CaseImage> CaseImages { get; set; } = new();
        public List<ExportedReport> ExportedReports { get; set; } = new();
        public List<GradCamImage> GradCamImages { get; set; } = new();

        // Static Properties
        public static string OutputBlob => $"output.pdf";
    }
}
