namespace MVC.Areas.Admin.Models;

public class PlaylistIndexViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string CreatorUserName { get; set; } = null!;
    public string CreatorId { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

