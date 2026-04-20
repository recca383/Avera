using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avera.Domain.Identity.Users;

namespace Avera.Domain.Application.Notifications
{
    public sealed class Notification
    {
        public Guid Id { get; set; }
        public NotificationType Type { get; set; }
        public string? Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public User? User { get; set; }
        public Guid UserId { get; set; }
    }
}
