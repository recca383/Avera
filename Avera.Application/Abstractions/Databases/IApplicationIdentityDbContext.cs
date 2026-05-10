using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avera.Domain.Identity.RoleClaims;
using Avera.Domain.Identity.Roles;
using Avera.Domain.Identity.ShareLinks;
using Avera.Domain.Identity.SubscriptionPlans;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.TenantSubscriptions;
using Avera.Domain.Identity.UserClaims;
using Avera.Domain.Identity.UserLogins;
using Avera.Domain.Identity.UserRoles;
using Avera.Domain.Identity.Users;
using Avera.Domain.Identity.UserTokens;
using Microsoft.EntityFrameworkCore;

namespace Avera.Application.Abstractions.Databases
{
    public interface IApplicationIdentityDbContext
    {
        DbSet<User> Users {get; set; }
        DbSet<Role> Roles {get; set; }
        DbSet<UserRole> UserRoles { get; set; }
        DbSet<RoleClaim> RoleClaims { get; set; }
        DbSet<UserLogin> UserLogins { get; set; }
        DbSet<UserToken> UserTokens { get; set; }
        DbSet<UserClaim> UserClaims { get; set; }
        DbSet<ShareLink> ShareLinks { get; set; }
        DbSet<Tenant> Tenants { get; set; }
        DbSet<TenantSubscription> TenantSubscriptions {get; set; }
        DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
