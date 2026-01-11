using DAL.Models.Enums;

namespace Business.DTOs;

public class OrderWithUsersDto
{
    public required OrderDto Order { get; set; }
    public required string OrdererName { get; set; }
    public required string CreatorName { get; set; }
}

