using Avera.Infrastructure.Database.Application;
using Avera.Infrastructure.Database.Identity;
using Microsoft.EntityFrameworkCore;

namespace Avera.WebApi.Extensions
{
    public static class MigrationExtension
    {
        public static async void MigrateIdentity(this WebApplication app)
        {
            using(var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var identityContext = services.GetRequiredService<IdentityDbContext>();

                    await identityContext.Database.MigrateAsync();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }            
        }    

        public static async void MigrateApplication(this WebApplication app)
        {
            using(var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var identityContext = services.GetRequiredService<ApplicationDbContext>();

                    await identityContext.Database.MigrateAsync();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }            
        }    
    }
}