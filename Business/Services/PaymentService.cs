using Business.DTOs;
using Business.Mappers;
using DAL.Data;
using DAL.Models;
using DAL.Models.Enums;
using ErrorOr;
using Infra.Repository;

namespace Business.Services;

public class PaymentService : IPaymentService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IGiftCardCodeRepository _giftCardCodeRepository;
    private readonly AppDbContext _dbContext;
    private readonly OrderMapper _orderMapper = new();
    private readonly SubscriptionMapper _subscriptionMapper = new();

    public PaymentService(
        IOrderRepository orderRepository,
        ISubscriptionRepository subscriptionRepository,
        IUserRepository userRepository,
        IGiftCardCodeRepository giftCardCodeRepository,
        AppDbContext dbContext)
    {
        _orderRepository = orderRepository;
        _subscriptionRepository = subscriptionRepository;
        _userRepository = userRepository;
        _giftCardCodeRepository = giftCardCodeRepository;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<PaymentResultDto>> ProcessSubscriptionPaymentAsync(PaymentProcessDto dto)
    {
        // Validate users exist
        var creator = await _userRepository.GetByIdAsync(dto.CreatorId);
        if (creator is null)
        {
            return Error.NotFound("Creator.NotFound", "Creator not found.");
        }

        var orderer = await _userRepository.GetByIdAsync(dto.OrdererId);
        if (orderer is null)
        {
            return Error.NotFound("Orderer.NotFound", "Orderer not found.");
        }

        // Can't subscribe to yourself
        if (dto.OrdererId == dto.CreatorId)
        {
            return Error.Validation("Payment.SelfSubscription", "You cannot subscribe to yourself.");
        }

        // Calculate price based on timeframe
        var basePrice = creator.PricePerMonth ?? 0;
        var originalAmount = CalculateAmount(basePrice, dto.Timeframe);

        // Validate and apply gift card if provided
        decimal discount = 0;
        GiftCardCode? giftCardCode = null;

        if (!string.IsNullOrWhiteSpace(dto.GiftCardCode))
        {
            var giftCardResult = await ValidateGiftCardCodeInternalAsync(dto.GiftCardCode);
            if (giftCardResult.IsError)
            {
                return giftCardResult.Errors;
            }

            giftCardCode = giftCardResult.Value;
            discount = giftCardCode.GiftCard.PriceReduction;
        }

        var finalAmount = Math.Max(0, originalAmount - discount);

        var order = new Order
        {
            Id = default,
            OrdererId = dto.OrdererId,
            CreatorId = dto.CreatorId,
            Amount = finalAmount,
            Status = OrderStatus.Completed, // Fake payment - always succeeds
            Orderer = orderer,
            Creator = creator,
            CreatedAt = default,
            UpdatedAt = default
        };

        // Mark gift card code as used if applied
        if (giftCardCode is not null)
        {
            giftCardCode.Used = true;
            giftCardCode.Order = order;
            await _giftCardCodeRepository.UpdateAsync(giftCardCode);
        }

        // Create subscription
        var timestamp = DateTime.UtcNow;
        var subscription = new Subscription
        {
            Id = default,
            OrdererId = dto.OrdererId,
            Orderer = orderer,
            CreatorId = dto.CreatorId,
            Creator = creator,
            Active = true,
            Timeframe = dto.Timeframe,
            SubscribedAt = timestamp,
            LastRenewedAt = timestamp,
            ExpiresAt = timestamp + GetTimeframeDuration(dto.Timeframe),
            CreatedAt = default,
            UpdatedAt = default
        };
        await _orderRepository.CreateAsync(order);
        await _subscriptionRepository.CreateAsync(subscription);

        await _dbContext.SaveChangesAsync();

        return new PaymentResultDto
        {
            Success = true,
            Order = _orderMapper.Map(order),
            Subscription = _subscriptionMapper.Map(subscription),
            OriginalAmount = originalAmount,
            FinalAmount = finalAmount,
            DiscountApplied = discount,
            GiftCardCodeUsed = dto.GiftCardCode
        };
    }

    public async Task<ErrorOr<decimal>> ValidateGiftCardCodeAsync(string code)
    {
        var result = await ValidateGiftCardCodeInternalAsync(code);
        if (result.IsError)
        {
            return result.Errors;
        }
        
        return result.Value.GiftCard.PriceReduction;
    }

    private async Task<ErrorOr<GiftCardCode>> ValidateGiftCardCodeInternalAsync(string code)
    {
        var giftCardCode = await _giftCardCodeRepository.GetByCodeAsync(code);
        
        if (giftCardCode is null)
        {
            return Error.NotFound("GiftCardCode.NotFound", "Gift card code not found.");
        }

        // Load the GiftCard if not already loaded
        await _dbContext.Entry(giftCardCode).Reference(g => g.GiftCard).LoadAsync();

        if (giftCardCode.Used)
        {
            return Error.Validation("GiftCardCode.AlreadyUsed", "This gift card code has already been used.");
        }

        var now = DateTime.UtcNow;
        if (now < giftCardCode.GiftCard.ValidFrom)
        {
            return Error.Validation("GiftCardCode.NotYetValid", "This gift card is not yet valid.");
        }

        if (now > giftCardCode.GiftCard.ValidTo)
        {
            return Error.Validation("GiftCardCode.Expired", "This gift card has expired.");
        }

        return giftCardCode;
    }

    private static decimal CalculateAmount(decimal monthlyPrice, SubscriptionTimeframe timeframe)
    {
        return timeframe switch
        {
            SubscriptionTimeframe.Month => monthlyPrice,
            SubscriptionTimeframe.HalfYear => monthlyPrice * 6 * 0.9m, // 10% discount for half year
            SubscriptionTimeframe.Year => monthlyPrice * 12 * 0.8m,   // 20% discount for full year
            _ => monthlyPrice
        };
    }

    private static TimeSpan GetTimeframeDuration(SubscriptionTimeframe timeframe)
    {
        return timeframe switch
        {
            SubscriptionTimeframe.Month => TimeSpan.FromDays(30),
            SubscriptionTimeframe.HalfYear => TimeSpan.FromDays(182),
            SubscriptionTimeframe.Year => TimeSpan.FromDays(365),
            _ => TimeSpan.FromDays(30)
        };
    }
}

