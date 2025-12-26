namespace pv179.Models;

public class SearchViewModel
{
    public string Query { get; set; } = string.Empty;
    public string Category { get; set; } = "all";
}

public class SearchResultsViewModel<T>
{
    public string Query { get; set; } = string.Empty;
    public List<T> Results { get; set; } = new();
    public string Category { get; set; } = string.Empty;
}

public class SearchAllResultsViewModel
{
    public string Query { get; set; } = string.Empty;
    public List<Business.DTOs.VideoDto> Videos { get; set; } = new();
    public List<Business.DTOs.PlaylistDto> Playlists { get; set; } = new();
    public List<Business.DTOs.UserDto> Creators { get; set; } = new();
}
