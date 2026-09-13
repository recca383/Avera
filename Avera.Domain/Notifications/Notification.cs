using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avera.Domain.Application.Notifications
{
    public sealed class Notification
    {
        public Guid Id { get; set; }

        public User User { get; set; }

        public Guid UserId { get; set; }

        public Tenant Tenant { get; set; }

        public Guid? TenantId { get; set; }

        public string Type { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string? ResourceId { get; set; }

        public bool IsRead { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset? ReadAt { get; set; }

    }
}
