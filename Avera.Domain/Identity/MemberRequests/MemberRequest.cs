using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using System.Runtime.CompilerServices;

namespace Avera.Domain.Identity.MemberRequests
{
    public class MemberRequest
    {
        public MemberRequest(Guid UserRequestedId, Guid TenantRequestedToId)
        {
            Id = Guid.NewGuid();
            Status = MemberRequestStatus.Pending;
            ReviewedAt = null;
            ReviewedByUserId = null;

            UserId = UserRequestedId;
            TenantId = TenantRequestedToId;
        }

        public Guid Id { get; set; }
        public MemberRequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public Guid? ReviewedByUserId { get; set; }

        // Navigation Attributes

        public User? User { get; set; }
        public Guid UserId { get; set; }
        public Tenant? Tenant { get; set; }
        public Guid TenantId { get; set; }

        // Ef Core
        public MemberRequest() { }

        // Static Methods
        public void Approve(Guid reviewerId)
        {

            Status = MemberRequestStatus.Approved;

            ReviewedByUserId = reviewerId;

        }

        public void Reject(Guid reviewerId)
        {
            Status = MemberRequestStatus.Rejected;


            ReviewedByUserId = reviewerId;
        }
    }
}