namespace DAL.Models;

public abstract class BaseEntity
{
    public required int Id { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}
