using System;

namespace Avera.Application.Cases.Notifications
{
    public sealed record NewCaseResultNotification(
        Guid CaseId,
        Guid CreatedByUserId,
        string CreatedByFirstName,
        string CreatedByLastName,
        Guid TenantId,
        DateTime CreatedAt
    );
}
