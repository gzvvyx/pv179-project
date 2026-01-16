using Business.DTOs;

namespace pv179.Models;

public class HomeFeedViewModel
{
    public List<VideoDto> Videos { get; set; } = new();
    public bool HasSubscriptions { get; set; }
}
