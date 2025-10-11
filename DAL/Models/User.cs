using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DAL.Models;

public class User : IdentityUser
{
    [InverseProperty(nameof(Video.Creator))]
    public ICollection<Video> Videos { get; set; } = new List<Video>();

    [InverseProperty(nameof(Playlist.Creator))]
    public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();

    [InverseProperty(nameof(Comment.Author))]
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    [InverseProperty(nameof(Order.Orderer))]
    public ICollection<Order> OrdersPlaced { get; set; } = new List<Order>();

    [InverseProperty(nameof(Order.Creator))]
    public ICollection<Order> OrdersReceived { get; set; } = new List<Order>();

    [InverseProperty(nameof(Subscription.Orderer))]
    public ICollection<Subscription> SubscriptionsPurchased { get; set; } = new List<Subscription>();

    [InverseProperty(nameof(Subscription.Creator))]
    public ICollection<Subscription> SubscriptionsOffered { get; set; } = new List<Subscription>();
}
