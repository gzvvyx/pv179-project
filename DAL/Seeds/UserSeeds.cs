using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Seeds;

public static class UserSeeds
{
    public static readonly User UserAlexGomez = new()
    {
        Id = "e8d6f9a3-4a8f-4527-bc75-bb8ae1f3ae72",
        UserName = "alex.gomez",
        NormalizedUserName = "ALEX.GOMEZ",
        Email = "alex.gomez@example.com",
        NormalizedEmail = "ALEX.GOMEZ@EXAMPLE.COM",
        PasswordHash = "4e3ed1f4250f8f2b21806f0f1c8c45cbcdaaa18c55072df09fae1c988d647b52",
        SecurityStamp = "7c4ded1f-17f2-44d4-bf54-1b8a9f6e9b54",
        ConcurrencyStamp = "1c181adc-59df-4510-a828-bd1185300f45",
        PhoneNumber = "+14155550111",
        PhoneNumberConfirmed = true,
        EmailConfirmed = true
    };

    public static readonly User UserMiaChen = new()
    {
        Id = "9fa1db08-5d4f-49bf-a2a3-a963c7d1c61c",
        UserName = "mia.chen",
        NormalizedUserName = "MIA.CHEN",
        Email = "mia.chen@example.com",
        NormalizedEmail = "MIA.CHEN@EXAMPLE.COM",
        PasswordHash = "f67a5c4d9fb3e1c89a3c772262c298f1a6df26c7d6eecd5f15d8b1e8721f3529",
        SecurityStamp = "d86300c0-36b8-4de9-89b5-ca5ef1d3ffb5",
        ConcurrencyStamp = "1c11a07d-4bfc-4712-92a7-d9b703d50695",
        PhoneNumber = "+14155550127",
        PhoneNumberConfirmed = true,
        EmailConfirmed = true
    };

    public static readonly User UserLukaNovak = new()
    {
        Id = "9910f947-0f76-42ac-87cf-6b4c786dcbbb",
        UserName = "luka.novak",
        NormalizedUserName = "LUKA.NOVAK",
        Email = "luka.novak@example.com",
        NormalizedEmail = "LUKA.NOVAK@EXAMPLE.COM",
        PasswordHash = "3bc8651d3256b9c1aee971ff3b3736d4c9bb976ba9b0144ebf5efe8211b697e4",
        SecurityStamp = "a197c17c-2e7f-4e4d-b0db-afdfc9f73ca6",
        ConcurrencyStamp = "a29fb90d-55d5-4b53-a6be-2eb3b9cb506d",
        PhoneNumber = "+38640111222",
        PhoneNumberConfirmed = true,
        EmailConfirmed = true
    };

    public static readonly User UserSaraIbrahim = new()
    {
        Id = "126c948e-5605-4ffc-93bb-0ab96af2898a",
        UserName = "sara.ibrahim",
        NormalizedUserName = "SARA.IBRAHIM",
        Email = "sara.ibrahim@example.com",
        NormalizedEmail = "SARA.IBRAHIM@EXAMPLE.COM",
        PasswordHash = "ac10a54045bdaaa0aa71447579ed6b1f40fd5c4f2f1fa3526f7c678b30f5d942",
        SecurityStamp = "d58ac48b-bf27-4a4f-95ab-663b2377074a",
        ConcurrencyStamp = "cb732fd5-74f0-48ed-bf8f-036eaaef650a",
        PhoneNumber = "+201078551234",
        PhoneNumberConfirmed = true,
        EmailConfirmed = true
    };

    public static readonly User UserNoahSingh = new()
    {
        Id = "6b3c8e66-3f5b-4e9f-a6d9-156b1f1a7ef3",
        UserName = "noah.singh",
        NormalizedUserName = "NOAH.SINGH",
        Email = "noah.singh@example.com",
        NormalizedEmail = "NOAH.SINGH@EXAMPLE.COM",
        PasswordHash = "7b0cf6bd05bc6b375094a27dc67edaf3958856fa98852f27eacbbf5242954442",
        SecurityStamp = "3fa3d065-e14f-4f83-bd04-311517383272",
        ConcurrencyStamp = "08f9fb00-88a4-4ec4-9bf3-044499a3c375",
        PhoneNumber = "+919876543210",
        PhoneNumberConfirmed = true,
        EmailConfirmed = true
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
        ResetNavigationProperties(UserAlexGomez);
        ResetNavigationProperties(UserMiaChen);
        ResetNavigationProperties(UserLukaNovak);
        ResetNavigationProperties(UserSaraIbrahim);
        ResetNavigationProperties(UserNoahSingh);
    }

    public static void Seed(this ModelBuilder builder)
    {
        builder.Entity<User>().HasData(
            UserAlexGomez,
            UserMiaChen,
            UserLukaNovak,
            UserSaraIbrahim,
            UserNoahSingh
        );
    }
}
