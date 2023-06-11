using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    // This class isn't for pushing, just for creating migrations
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isDev)
        {
            using( var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isDev);
            }
        }

        private static void SeedData(AppDbContext context, bool isDev)
        {
            if(!isDev)
            {
                Console.WriteLine("--> Attempting to apply migrations...");
                try
                {
                    // context.Database.Migrate();
                    context.Database.Migrate();
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"--> Could not run migrations: {ex.Message}");
                }
            }

            if(!context.Platforms.Any())
            {
                Console.WriteLine("---> Seeding Data ...");

                context.Platforms.AddRange(
                    new Platform() {Name="Dot Net", Publisher="Microsoft", Cost="Free"},
                    new Platform() {Name="PostgresSQL", Publisher="Not Sure", Cost="Free"},
                    new Platform() {Name="Kubernetes", Publisher="Cloud Native Computing Foundation", Cost="Free"}
                );

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("---> We already have data");
            }
        }

    }
}