using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avera.Domain.Application.ExportedReports
{
    public sealed class ExportedReport
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public string FileUrl { get; set; }
        public string StorageKey { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
