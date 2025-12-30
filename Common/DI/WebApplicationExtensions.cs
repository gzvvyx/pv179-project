using DAL.Data;
using DAL.Models;
using DAL.Seeds;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.DI;

public static class WebApplicationExtensions
{
    public static async Task MigrateAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        await db.Database.MigrateAsync();

        await RoleSeeds.SeedRolesAsync(roleManager);

        if (app.Environment.IsDevelopment())
        {
            await BogusSeeder.SeedAsync(db, userManager, roleManager);
        }

        await UserSeeds.SeedAdminAsync(userManager);
    }
}


