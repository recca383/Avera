using Avera.Domain.Application.Cases;
using Avera.Domain.Application.Notifications;
using Avera.Infrastructure.Identity.Tenants;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avera.Infrastructure.Identity.Users
{
    public class User : IdentityUser<Guid>
    {
        // Navigation 
        public List<Notification> Notifications { get; set; } = new();
        public List<Case> Cases { get; set; } = new();
        public Tenant? Tenant { get; set; }
        public Guid TenantId { get; set; }
        
    }
}
