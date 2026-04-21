
using Avera.Domain.Identity.ShareLinks;
using Avera.Domain.Identity.TenantSubscriptions;
using Avera.Domain.Identity.Users;

namespace Avera.Domain.Identity.Tenants
{
    public class Tenant{
        public Guid Id { get; set; }
        public string Name { get; set; } = "No Display Name";
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public TenantSubscription TenantSubscription { get; set; } = new();
        public List<User> Users { get; set; } = new();
        public List<ShareLink> ShareLinks { get; set; } = new();
    }
    
}