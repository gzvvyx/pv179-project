using DAL.Models.Enums;

namespace DAL.Models;



public class AuditLog : BaseEntity
{
    public string? UserId { get; set; }
    public required string Table { get; set; }
    public required int EntityId { get; set; }
    public required AuditAction Action { get; set; }
}
