using Avera.Application.Abstractions.Services;
using Avera.Application.Abstractions.Authentication;
using SharedKernel;
using Avera.Application.Abstractions.Messaging;

namespace Avera.Application.Users.SetDailyLimit;

internal sealed class SetUserDailyLimitCommandHandler(IAdminService adminService, IUserContext userContext) : ICommandHandler<SetUserDailyLimitCommand>
{
    public async Task<Result> Handle(SetUserDailyLimitCommand command, CancellationToken cancellationToken)
    {
        // Delegate to admin service which enforces tenant scoping and validation
        return await adminService.SetUserDailyCaseLimitAsync(command.UserId, command.DailyLimit, cancellationToken);
    }
}
