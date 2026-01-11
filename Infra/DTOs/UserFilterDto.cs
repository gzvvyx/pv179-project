namespace Infra.DTOs
{
    public class UserFilterDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        public string? UserName { get; set; }
        public string? Email { get; set; }

        public string? SortBy { get; set; } = "UserName";
        public bool SortDescending { get; set; } = false;
    }
}
