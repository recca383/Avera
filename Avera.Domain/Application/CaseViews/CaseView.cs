namespace Avera.Domain.Application.CaseViews;

public class CaseView
{
    public Guid CaseId { get; set; }
    public Guid UserId { get; set; }
    public DateTime ViewedAt { get; set; }
}
