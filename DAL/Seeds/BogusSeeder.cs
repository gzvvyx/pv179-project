using Bogus;
using Microsoft.EntityFrameworkCore;
using DAL.Data;
using DAL.Models;

namespace DAL.Seeds;

public static class BogusSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        // Only seed if empty to avoid duplicates
        if (await db.Users.AnyAsync())
            return;

        Bogus.Randomizer.Seed = new Random(123);

        await BogusUserSeeds.SeedAsync(db);
        await BogusVideoSeeds.SeedAsync(db);
        await BogusPlaylistSeeds.SeedAsync(db);
        await BogusOrderSeeds.SeedAsync(db);
        await BogusSubscriptionSeeds.SeedAsync(db);
        await BogusCommentSeeds.SeedAsync(db);
    }
}