using Avera.Domain.Application.Cases;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Avera.Domain.Notifications
{
    public record NotificationErrors : Error
    {
        private NotificationErrors(string code, string message, ErrorType errorType) : base(code, message, errorType)
        {
        }

        public static NotificationErrors NotificationNotFound => new NotificationErrors(
            "Notification.NotFound",
            "The Notification is not existing in the database.",
            ErrorType.NotFound);
    }
}
