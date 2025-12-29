namespace pv179.Models;

public record PlaylistItemVM(
    int Id,
    string Name,
    string? Description,
    string Creator
);
