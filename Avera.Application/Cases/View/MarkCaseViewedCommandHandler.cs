using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Authentication;
using SharedKernel;
using Microsoft.EntityFrameworkCore;
using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.Cases.View;

internal sealed class MarkCaseViewedCommandHandler : ICommandHandler<MarkCaseViewedCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUserContext _userContext;

    public MarkCaseViewedCommandHandler(IApplicationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<Result> Handle(MarkCaseViewedCommand command, CancellationToken cancellationToken)
    {
        if (_userContext.TenantId == null)
            return Result.Failure(Avera.Domain.Identity.Tenants.TenantErrors.NotMember);

        // Verify case exists and belongs to tenant
        var caseEntity = await _dbContext.Cases.FirstOrDefaultAsync(c => c.Id == command.CaseId, cancellationToken);

        if (caseEntity == null)
            return Result.Failure(Avera.Domain.Application.Cases.CaseErrors.CaseNotFound);

        if (caseEntity.TenantId != _userContext.TenantId)
            return Result.Failure(Avera.Domain.Identity.Tenants.TenantErrors.UserNotInTenant);

        // Upsert CaseView
        var existing = await _dbContext.CaseViews.FindAsync(new object[] { command.CaseId, _userContext.UserId }, cancellationToken);

        if (existing != null)
        {
            existing.ViewedAt = DateTime.UtcNow;
        }
        else
        {
            var view = new Avera.Domain.Application.CaseViews.CaseView
            {
                CaseId = command.CaseId,
                UserId = _userContext.UserId,
                ViewedAt = DateTime.UtcNow
            };

            await _dbContext.CaseViews.AddAsync(view, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
