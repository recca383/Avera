using Avera.Domain.Identity.MemberRequests;
using Avera.Domain.Identity.TenantSubscriptions;
using Avera.Domain.Identity.Users;
using Avera.Infrastructure.Identity.Tenants;

namespace Avera.Domain.Identity.Tenants
{
    public class Tenant{
        public Guid Id { get; set; }
        public string Name { get; set; } = "No Display Name";
        public TenantStatus Status { get; set; }
        public required string InviteCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MemberCountLimit { get; set; } = 5;

        // Navigation Properties
        public List<TenantSubscription> TenantSubscriptions { get; set; } = new();
        public List<User> Users { get; set; } = new();
        public List<MemberRequest> MemberRequests { get; set; } = new();
    }
    
}