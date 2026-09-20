using Avera.Application.Cases.Create;
using SharedKernel;

namespace Avera.Application.Abstractions.Queues;

public interface ICaseCreationQueue
{
    Task<Result<Domain.Application.Cases.Case>> EnqueueAsync(CreateCaseCommand command, Guid callerUserId, Guid? callerTenantId, CancellationToken cancellationToken = default);
}
