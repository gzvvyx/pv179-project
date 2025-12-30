using DAL.Models;
using DAL.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DAL.Seeds;

public static class UserSeeds
{
    public static readonly User AdminUser = new()
    {
        Id = "00000000-0000-0000-0000-000000000001",
        UserName = "admin",
        NormalizedUserName = "ADMIN",
        Email = "admin@example.com",
        NormalizedEmail = "ADMIN@EXAMPLE.COM",
        EmailConfirmed = true,
        PhoneNumberConfirmed = false
    };

    private static void ResetNavigationProperties(User user)
    {
        user.Videos = [];
        user.Playlists = [];
        user.Comments = [];
        user.OrdersPlaced = [];
        user.OrdersReceived = [];
        user.SubscriptionsPurchased = [];
        user.SubscriptionsOffered = [];
    }

    public static void LoadLists()
    {
        ResetNavigationProperties(AdminUser);
    }

    public static async Task SeedAdminAsync(UserManager<User> userManager)
    {
        var existingAdmin = await userManager.FindByNameAsync(AdminUser.UserName!);
        if (existingAdmin != null)
        {
            return;
        }

        var result = await userManager.CreateAsync(AdminUser, "Test123!");
        if (!result.Succeeded)
        {
            throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        var adminRoleResult = await userManager.AddToRoleAsync(AdminUser, AppRoles.Admin);
        if (!adminRoleResult.Succeeded)
        {
            throw new Exception($"Failed to assign Admin role: {string.Join(", ", adminRoleResult.Errors.Select(e => e.Description))}");
        }

        var userRoleResult = await userManager.AddToRoleAsync(AdminUser, AppRoles.User);
        if (!userRoleResult.Succeeded)
        {
            throw new Exception($"Failed to assign User role: {string.Join(", ", userRoleResult.Errors.Select(e => e.Description))}");
        }
    }
}
