namespace Infra.DTOs
{
    public class UserFilterDto
    {
        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        
        // Search
        public string? UserName { get; set; }
        public string? Email { get; set; }
        
        // Sorting
        public string? SortBy { get; set; } = "UserName";
        public bool SortDescending { get; set; } = false;
    }
}
