using Avera.Application.Abstractions.Messaging;
using Avera.Application.ML.Health;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Avera.Application.Abstractions.ML.Health
{
    internal sealed class GetMLHealth(
        IMLService mLService,
        ILogger<GetMLHealth> logger) 
    : ICommandHandler<GetMLHealthCommand, GetMLHealthResponse>
    {
        public async Task<Result<GetMLHealthResponse>> Handle(GetMLHealthCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var health = await mLService.GetMLHealthAsync(cancellationToken);
                logger.LogInformation("ML Service Health: Status: {Status}, Version: {Version}", health.Status, health.Version);
                return Result.Success(health);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while getting ML service health");
                return Result.Failure<GetMLHealthResponse>(new Error("MLHealth.Failure", "An error occurred while checking ML service health.", ErrorType.Failure));
            }

        }
    }
}