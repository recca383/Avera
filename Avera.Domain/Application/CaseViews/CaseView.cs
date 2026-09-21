using Avera.Domain.Application.Cases;
using Avera.Domain.Identity.Users;

namespace Avera.Domain.Application.CaseViews;

public class CaseView
{
    public Guid CaseId { get; set; }
    public Case Case { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public DateTime ViewedAt { get; set; }
}
