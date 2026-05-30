using Avera.Application.Abstractions.Databases;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.PatchIsViewed;
using Avera.Domain.Application.Cases;
using SharedKernel;

namespace Avera.Application.Cases.PatchIsViewed
{
    internal sealed class PatchIsViewedCaseCommandHandler
    (IApplicationDbContext applicationDbContext) : ICommandHandler<PatchIsViewedCaseCommand, Guid>
    {
        public Task<Result<Guid>> Handle(PatchIsViewedCaseCommand command, CancellationToken cancellationToken)
        {
            var selectedCase = applicationDbContext.Notifications.FirstOrDefault(c => c.Id == command.Id);

            if (selectedCase is null)
            {
                return Task.FromResult(Result.Failure<Guid>(CaseErrors.CaseNotFound));
            }

            // selectedCase.IsViewed = command.IsViewed;
            // applicationDbContext.Cases.Update(selectedCase);
            // applicationDbContext.SaveChanges();

            return Task.FromResult(Result.Success(command.Id));
        }
    }
}