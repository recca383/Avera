using Avera.Domain.Application.Notifications;
using Avera.Domain.Identity.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Avera.Infrastructure.Database.Application.Configurations
{
    internal class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasOne<User>(u => u.User).WithMany(n => n.Notifications).HasForeignKey(u => u.UserId);

            builder.HasKey(n => n.UserId);

            builder.HasAlternateKey(n => n.TenantId);

            

        }
    }
}