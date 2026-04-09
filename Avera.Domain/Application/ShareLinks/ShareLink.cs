using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avera.Domain.Application.ShareLinks
{
    public sealed class ShareLink
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
