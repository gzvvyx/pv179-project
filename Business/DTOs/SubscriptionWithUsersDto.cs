namespace Business.DTOs;

public class SubscriptionWithUsersDto
{
    public required SubscriptionDto Subscription { get; set; }
    public required string OrdererName { get; set; }
    public required string CreatorName { get; set; }
}

