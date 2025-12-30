namespace MVC.Areas.Admin.Models;

public class VideoIndexViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Url { get; set; } = null!;
    public string CreatorUserName { get; set; } = null!;
    public string CreatorId { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

