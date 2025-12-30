using Bogus;
using DAL.Models;
using DAL.Data;
using DAL.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAL.Seeds;

public static class BogusUserSeeds
{
    public static async Task SeedAsync(AppDbContext db, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, int numberOfUsers = 10)
    {
        if (await db.Users.AnyAsync())
        {
            return;
        }

        var faker = new Faker<User>("en")
                .RuleFor(u => u.UserName, f => f.Internet.UserName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.UserName))
                .RuleFor(u => u.EmailConfirmed, _ => true)
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.PhoneNumberConfirmed, _ => false);

        var users = faker.Generate(numberOfUsers);

        foreach (var user in users)
        {
            var result = await userManager.CreateAsync(user, "Test123!");
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create user {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            var roleResult = await userManager.AddToRoleAsync(user, AppRoles.User);
            if (!roleResult.Succeeded)
            {
                throw new Exception($"Failed to assign User role to user {user.UserName}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }
        }
    }
}
