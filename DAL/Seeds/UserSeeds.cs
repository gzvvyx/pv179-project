using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Seeds;

public static class UserSeeds
{
    public static readonly User User1 = new()
    {
        Id = "53e83d19-1acc-406e-b84d-ce4739a25809",
        UserName = "john_doe",
        Email = "email@email.com",
        PasswordHash = "0390f290ec23a40a1073ab5f10401fc673a033ee8caf6d9a2ea9616e1ccc0120"
    };

    public static void LoadLists()
    {
        User1.Videos = [];
        User1.Playlists = [];
        User1.Comments = [];
        User1.OrdersPlaced = [];
        User1.OrdersReceived = [];  
        User1.SubscriptionsPurchased = [];
        User1.SubscriptionsOffered = [];
    }

    public static void Seed(this ModelBuilder builder)
    {
        builder.Entity<User>().HasData(User1);
    }
}
