using Avera.Infrastructure.Identity.ShareLinks;
using Avera.Infrastructure.Identity.TenantSubscriptions;
using Avera.Infrastructure.Identity.Users;

namespace Avera.Infrastructure.Identity.Tenants
{
    public class Tenant{
        public Guid Id { get; set; }
        public string Name { get; set; } = "No Display Name";
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public List<TenantSubscription> TenantSubscriptions { get; set; } = new();
        public List<User> Users { get; set; } = new();
        public List<ShareLink> ShareLinks { get; set; } = new();
    }
    
}