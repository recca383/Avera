using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avera.Domain.Application.Cases
{
    public sealed class Case
    {
        public Guid Id { get; set; }
        public string CaseCode { get; set; }
        public string SubjectName { get; set; }
        public DocumentType Type { get; set; }
        public Priority Priority { get; set; }
        public string Notes { get; set; }
        public Status Status { get; set; }
        public Verdict Verdict { get; set; }
        public DateTime DeletedAt { get; set; }
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
