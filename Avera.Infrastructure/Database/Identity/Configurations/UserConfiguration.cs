using Avera.Domain.Application.Cases;
using Avera.Domain.Identity.Tenants;
using Avera.Domain.Identity.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avera.Infrastructure.Database.Identity.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.HasIndex(u => u.Id);
            builder.HasIndex(u => u.TenantId);
            
            builder.HasOne<Tenant>(u => u.Tenant)
                .WithMany(t => t.Users)
                .HasForeignKey(u => u.TenantId);
            
            builder.HasMany<Case>(u => u.Cases)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId);  
        }
    }
}