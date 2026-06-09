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
        public NotificationType Type { get; set; }
        public string? Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
