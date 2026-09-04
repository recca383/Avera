using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Messaging;
using Avera.Application.ML.Health;
using Avera.Domain.Application.Cases;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Avera.Application.Abstractions.ML.Health
{
    internal sealed class GetMLHealth(
        IMLService mLService,
        // Temporary Logger
        ILogger<GetMLHealth> logger,
        IUserContext userContext,
        UserManager<User> userManager) 
    : ICommandHandler<GetMLHealthCommand, GetMLHealthResponse>
    {
        public async Task<Result<GetMLHealthResponse>> Handle(GetMLHealthCommand command, CancellationToken cancellationToken)
        {
            if (userContext.TenantId == null)
                return Result.Failure<GetMLHealthResponse>(TenantErrors.NotMember);

            var user = await userManager.FindByIdAsync(userContext.UserId.ToString());

            if (user!.IsSuspended)
                return Result.Failure<GetMLHealthResponse>(UserErrors.IsSuspended);

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