using Avera.Domain.Application.Cases;
using Avera.Domain.Application.Notifications;
using Microsoft.AspNetCore.Identity;

namespace Avera.Domain.Identity.Users
{
    public class User : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsSuspended { get; set; } = false;
        // Optional per-user daily case creation cap. Null means no limit.
        public int? DailyCaseLimit { get; set; }

        // Navigation 
        public List<Notification> Notifications { get; set; } = new();
        public List<Case> Cases { get; set; } = new();
        public Guid? TenantId { get; set; }
        
    }
}
