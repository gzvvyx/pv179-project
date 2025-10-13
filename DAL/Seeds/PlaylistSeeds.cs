using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Seeds;

public static class PlaylistSeeds
{
    public static readonly Playlist PlaylistSports = new()
    {
        Id = 1,
        CreatorId = UserSeeds.UserMiaChen.Id,
        Name = "Amazing Sports",
        Description = "A playlist for amazing sports videos.",
        Creator = UserSeeds.UserMiaChen,
        Videos = new List<Video>
        {
            VideoSeeds.VideoAlexUrbanCycling,
            VideoSeeds.VideoSaraYogaFlow,
            VideoSeeds.VideoNoahDanceWorkshop
        },
        CreatedAt = new DateTime(2024, 4, 1, 19, 20, 50, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2024, 4, 10, 20, 40, 20, DateTimeKind.Utc)
    };

    public static readonly Playlist PlaylistWomanPower = new()
    {
        Id = 2,
        CreatorId = UserSeeds.UserSaraIbrahim.Id,
        Name = "WOMAN POWWAAA",
        Description = "Strong and powerful",
        Creator = UserSeeds.UserSaraIbrahim,
        Videos = new List<Video>
        {
            VideoSeeds.VideoMiaHomeAutomation,
            VideoSeeds.VideoSaraYogaFlow,
            VideoSeeds.VideoAlexUrbanCycling
        },
        CreatedAt = new DateTime(2024, 4, 10, 19, 20, 50, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2024, 5, 10, 15, 40, 20, DateTimeKind.Utc)
    };

    public static readonly Playlist PlaylistEntertainment = new()
    {
        Id = 3,
        CreatorId = UserSeeds.UserLukaNovak.Id,
        Name = "Too boring to be",
        Description = "Spinks",
        Creator = UserSeeds.UserLukaNovak,
        Videos = new List<Video>
        {
            VideoSeeds.VideoLukaDroneExploration,
            VideoSeeds.VideoSaraYogaFlow
        },
        CreatedAt = new DateTime(2025, 8, 7, 23, 40, 10, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2025, 10, 3, 9, 28, 20, DateTimeKind.Utc)
    };
    public static void LoadLists()
    {
    }

    public static void Seed(this ModelBuilder builder)
    {
        builder.Entity<Playlist>().HasData(
            CreateSeed(PlaylistSports),
            CreateSeed(PlaylistWomanPower),
            CreateSeed(PlaylistEntertainment)
        );
    }

    private static object CreateSeed(Playlist playlist)
    {
        return new
        {
            playlist.Id,
            playlist.CreatorId,
            playlist.Name,
            playlist.Description,
            playlist.CreatedAt,
            playlist.UpdatedAt
        };
    }
}
