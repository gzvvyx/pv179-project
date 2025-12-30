using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DAL.Seeds;

public static class BogusSeeder
{
    public static async Task SeedAsync(AppDbContext db, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Only seed if empty to avoid duplicates
        if (await db.Users.AnyAsync())
            return;

        Bogus.Randomizer.Seed = new Random(123);

        await BogusUserSeeds.SeedAsync(db, userManager, roleManager);
        await BogusVideoSeeds.SeedAsync(db);
        await BogusPlaylistSeeds.SeedAsync(db);
        await BogusOrderSeeds.SeedAsync(db);
        await BogusSubscriptionSeeds.SeedAsync(db);
        await BogusCommentSeeds.SeedAsync(db);
        await BogusCategorySeeds.SeedAsync(db);
        await BogusCategorySeeds.SeedVideoCategoriesAsync(db);
        await BogusGiftCardSeeds.SeedAsync(db);
        await BogusGiftCardCodeSeeds.SeedAsync(db);
    }
}