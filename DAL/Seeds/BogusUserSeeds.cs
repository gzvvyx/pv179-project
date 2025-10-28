using Bogus;
using DAL.Models;
using DAL.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAL.Seeds;

public static class BogusUserSeeds
{
    public static async Task SeedAsync(AppDbContext db, int numberOfUsers = 10)
    {
        if (await db.Users.AnyAsync())
        {
            return;
        }

        var passwordHasher = new PasswordHasher<User>();

        var faker = new Faker<User>("en")
                .RuleFor(u => u.Id, _ => Guid.NewGuid().ToString())
                .RuleFor(u => u.UserName, f => f.Internet.UserName())
                .RuleFor(u => u.NormalizedUserName, (f, u) => u.UserName.ToUpperInvariant())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.UserName))
                .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email.ToUpperInvariant())
                .RuleFor(u => u.EmailConfirmed, _ => true)
                .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.PhoneNumberConfirmed, _ => false)
                .RuleFor(u => u.SecurityStamp, _ => Guid.NewGuid().ToString("N"))
                .RuleFor(u => u.ConcurrencyStamp, _ => Guid.NewGuid().ToString("N"))
                .FinishWith((f, u) =>
                {
                    u.PasswordHash = passwordHasher.HashPassword(u, "Test123!");
                });

        var users = faker.Generate(numberOfUsers);

        await db.Users.AddRangeAsync(users);
        await db.SaveChangesAsync();
    }
}
