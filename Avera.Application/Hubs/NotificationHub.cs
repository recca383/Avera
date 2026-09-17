using Avera.Application.Abstractions.Authentication;
using Avera.Application.Abstractions.Databases;
using Avera.Domain.Identity.Users;
using Microsoft.AspNetCore.Authorization;
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
            var userId = userContext.UserId;

            var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new ApplicationException("User not found");

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"user:{userId}");

            var tenantId = userContext.TenantId;

            if (tenantId.HasValue)
            { 
                var isAdmin = await userManager.IsInRoleAsync(user, "Admin");

                if (isAdmin)
                {
                    await Groups.AddToGroupAsync(
                        Context.ConnectionId,
                        $"tenant:{tenantId}:admins");
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(
            Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
