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
        public override async Task OnConnectedAsync()
        {
            var tenantId = userContext.TenantId;

            var user = await userManager.FindByIdAsync(userContext.UserId.ToString());

            if (user == null)
            {
                throw new Exception("User not Found");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{user.Id}");

            var isAdmin = await userManager.IsInRoleAsync(user, "Admin");

            if (isAdmin)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant:{tenantId}:admins");
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
