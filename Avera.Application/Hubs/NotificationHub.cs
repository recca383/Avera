using Avera.Application.Abstractions.Authentication;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SharedKernel;

namespace Avera.Application.Hubs
{
    public sealed class NotificationHub(
        UserManager<User> userManager,
        IUserContext userContext

        ) : Hub
    {
        public override async Task<Result> OnConnectedAsync()
        {
            var tenantId = userContext.TenantId;

            var user = await userManager.FindByIdAsync(userContext.UserId.ToString());

            if (user == null)
            {
                return Result.Failure(UserErrors.UserNotFound);
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{user.Id}");

            var isAdmin = await userManager.IsInRoleAsync(user, "Admin");

            if (isAdmin)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant:{tenantId}:admins");
            }

            


            return Result.Success();
        }

        public override async Task<Result> OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);

            return Result.Success(exception == null ? null : new Error("Hub.DisconnectedError", exception.Message, ErrorType.ServerError));
        }
    }
}
