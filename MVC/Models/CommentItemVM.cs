namespace pv179.Models;

public record CommentItemVM(
    int Id,
    string Content,
    DateTime CreatedAt,
    string Author
);
