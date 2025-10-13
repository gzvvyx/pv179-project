using System;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Seeds;

public static class VideoSeeds
{
    public static readonly Video VideoAlexUrbanCycling = new()
    {
        Id = 1,
        CreatorId = UserSeeds.UserAlexGomez.Id,
        Title = "Urban Cycling Masterclass",
        Description = "Alex shares techniques for safer city rides with tips on pacing, gear, and visibility.",
        Url = "https://cdn.example.com/videos/alex-urban-cycling.mp4",
        ThumbnailUrl = "https://cdn.example.com/thumbnails/alex-urban-cycling.jpg",
        Creator = null!,
        CreatedAt = new DateTime(2024, 1, 5, 8, 30, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2024, 2, 2, 11, 45, 0, DateTimeKind.Utc)
    };

    public static readonly Video VideoMiaHomeAutomation = new()
    {
        Id = 2,
        CreatorId = UserSeeds.UserMiaChen.Id,
        Title = "Build a Home Robot Barista",
        Description = "Mia walks through designing and coding a coffee-serving robot using off-the-shelf parts.",
        Url = "https://cdn.example.com/videos/mia-robot-barista.mp4",
        ThumbnailUrl = "https://cdn.example.com/thumbnails/mia-robot-barista.jpg",
        Creator = null!,
        CreatedAt = new DateTime(2024, 3, 12, 15, 5, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2024, 3, 20, 9, 20, 0, DateTimeKind.Utc)
    };

    public static readonly Video VideoLukaDroneExploration = new()
    {
        Id = 3,
        CreatorId = UserSeeds.UserLukaNovak.Id,
        Title = "Alpine Drone Photography Guide",
        Description = "Luka explores alpine valleys while teaching framing, exposure, and post-processing workflows.",
        Url = "https://cdn.example.com/videos/luka-drone-alps.mp4",
        ThumbnailUrl = "https://cdn.example.com/thumbnails/luka-drone-alps.jpg",
        Creator = null!,
        CreatedAt = new DateTime(2023, 12, 18, 6, 50, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2024, 1, 10, 13, 10, 0, DateTimeKind.Utc)
    };

    public static readonly Video VideoSaraYogaFlow = new()
    {
        Id = 4,
        CreatorId = UserSeeds.UserSaraIbrahim.Id,
        Title = "Mindful Morning Yoga Flow",
        Description = "Sara leads a calming 20-minute practice blending breath work with gentle strength building.",
        Url = "https://cdn.example.com/videos/sara-morning-yoga.mp4",
        ThumbnailUrl = "https://cdn.example.com/thumbnails/sara-morning-yoga.jpg",
        Creator = null!,
        CreatedAt = new DateTime(2024, 2, 8, 5, 15, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2024, 2, 15, 7, 45, 0, DateTimeKind.Utc)
    };

    public static readonly Video VideoNoahDanceWorkshop = new()
    {
        Id = 5,
        CreatorId = UserSeeds.UserNoahSingh.Id,
        Title = "Bollywood Fusion Dance Workshop",
        Description = "Noah blends classical Kathak steps with hip hop grooves in a high-energy routine.",
        Url = "https://cdn.example.com/videos/noah-bollywood-fusion.mp4",
        ThumbnailUrl = "https://cdn.example.com/thumbnails/noah-bollywood-fusion.jpg",
        Creator = null!,
        CreatedAt = new DateTime(2024, 4, 1, 17, 40, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2024, 4, 18, 19, 5, 0, DateTimeKind.Utc)
    };

    public static void LoadLists()
    {
    }

    public static void Seed(this ModelBuilder builder)
    {
        builder.Entity<Video>().HasData(
            CreateSeed(VideoAlexUrbanCycling),
            CreateSeed(VideoMiaHomeAutomation),
            CreateSeed(VideoLukaDroneExploration),
            CreateSeed(VideoSaraYogaFlow),
            CreateSeed(VideoNoahDanceWorkshop)
        );
    }

    private static object CreateSeed(Video video)
    {
        return new
        {
            video.Id,
            video.CreatorId,
            video.Title,
            video.Description,
            video.Url,
            video.ThumbnailUrl,
            video.CreatedAt,
            video.UpdatedAt
        };
    }
}
