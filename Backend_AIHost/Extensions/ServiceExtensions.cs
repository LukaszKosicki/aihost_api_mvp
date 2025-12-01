using Backend_AIHost.Data;
using Backend_AIHost.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend_AIHost.Extensions
{
    public static class ServiceExtensions
    {
        // Konfiguracja bazy danych
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));
            return services;
        }

        // Konfiguracja Identity
        public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            return services;
        }

        // Asynchroniczne seedowanie danych
        public static async Task SeedDataAsync(this IServiceProvider serviceProvider)
        {
           
            await using var scope = serviceProvider.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var adminPassword = config["ADMIN_PASSWORD"]; 

            // Migracja bazy
            await db.Database.MigrateAsync();

            // Seed AI Models
            if (!db.AIModels.Any())
            {
                SeedData.SeedAIModel(db); // zakładam, że SeedData.SeedAIModel jest synchroniczne
                await db.SaveChangesAsync();
            }

            // Seed ról
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Dodanie admina
            var adminEmail = "kontakt@lk-soft.info";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = "admin",
                    Email = adminEmail
                };
                await userManager.CreateAsync(adminUser, adminPassword);
            }

            // Przypisanie roli admina
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
