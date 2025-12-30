using Microsoft.AspNetCore.Identity;
using DAL.Authorization;

namespace DAL.Seeds;

public static class RoleSeeds
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync(AppRoles.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(AppRoles.Admin));
        }

        if (!await roleManager.RoleExistsAsync(AppRoles.User))
        {
            await roleManager.CreateAsync(new IdentityRole(AppRoles.User));
        }
    }
}

