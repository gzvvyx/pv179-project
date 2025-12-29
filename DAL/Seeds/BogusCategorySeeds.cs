using Bogus;
using DAL.Data;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Seeds;

public static class BogusCategorySeeds
{
    private static readonly string[] CategoryNames = new[]
    {
        "Music",
        "Gaming",
        "Education",
        "Entertainment",
        "News",
        "Sports",
        "Technology",
        "Cooking",
        "Travel",
        "Comedy",
        "Science",
        "Fashion",
        "Health & Fitness",
        "DIY & Crafts",
        "Animation",
        "Documentary",
        "Reviews",
        "Tutorials",
        "Vlogs",
        "Podcasts"
    };

    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Categories.AnyAsync())
            return;

        var now = DateTime.UtcNow;
        var categories = CategoryNames.Select((name, index) => new Category
        {
            Id = default,
            Name = name,
            CreatedAt = now.AddDays(-30 + index),
            UpdatedAt = now.AddDays(-30 + index)
        }).ToList();

        db.Categories.AddRange(categories);
        await db.SaveChangesAsync();
    }

    public static async Task SeedVideoCategoriesAsync(AppDbContext db)
    {
        if (await db.VideoCategories.AnyAsync())
            return;

        var videos = await db.Videos.ToListAsync();
        var categories = await db.Categories.ToListAsync();

        if (!videos.Any() || !categories.Any())
            return;

        var faker = new Faker();
        var now = DateTime.UtcNow;
        var videoCategories = new List<VideoCategory>();

        foreach (var video in videos)
        {
            var numberOfCategories = faker.Random.Int(1, 3);
            var selectedCategories = faker.PickRandom(categories, numberOfCategories).ToList();

            for (int i = 0; i < selectedCategories.Count; i++)
            {
                var category = selectedCategories[i];
                videoCategories.Add(new VideoCategory
                {
                    Id = default,
                    VideoId = video.Id,
                    CategoryId = category.Id,
                    IsPrimary = i == 0,
                    CreatedAt = now,
                    UpdatedAt = now
                });
            }
        }

        db.VideoCategories.AddRange(videoCategories);
        await db.SaveChangesAsync();
    }
}
