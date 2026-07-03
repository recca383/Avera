using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avera.Infrastructure.Identity.Tenants;

namespace Avera.Infrastructure.Identity.InviteCodes
{
    public sealed class InviteCode
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public Guid TenantId { get; set; }
        public Tenant? Tenant { get; set; }

    }
}
