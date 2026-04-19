using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avera.Domain.Application.Cases;

namespace Avera.Domain.Application.ExportedReports
{
    public sealed class ExportedReport
    {
        public Guid Id { get; set; }
        public string? FileUrl { get; set; }
        public string? StorageKey { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public Guid CaseId { get; set; }
        public Case? Case { get; set; }
    }
}
