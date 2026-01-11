using API.DTOs;
using API.Extensions;
using Business.DTOs;
using Business.Services;
using DAL.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionController : ControllerBase
{

    private readonly ILogger<SubscriptionController> _logger;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IPaymentService _paymentService;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUserService;

    public SubscriptionController(
        ILogger<SubscriptionController> logger, 
        ISubscriptionService subscriptionService,
        IPaymentService paymentService,
        IUserService userService,
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _subscriptionService = subscriptionService;
        _paymentService = paymentService;
        _userService = userService;
        _currentUserService = currentUserService;
    }

    [HttpGet(Name = "GetSubscriptions")]
    public async Task<IEnumerable<SubscriptionDto>> Get()
    {
        return await _subscriptionService.GetAllAsync();
    }

    [HttpGet("with-users", Name = "GetSubscriptionsWithUsers")]
    public async Task<IEnumerable<SubscriptionWithUsersDto>> GetAllWithUsers()
    {
        return await _subscriptionService.GetAllWithUsersAsync();
    }

    [HttpGet("{id:int}", Name = "GetSubscriptionById")]
    public async Task<ActionResult<SubscriptionDto>> GetById(int id)
    {
        var subscription = await _subscriptionService.GetByIdAsync(id);

        if (subscription is null)
        {
            return NotFound();
        }

        return Ok(subscription);
    }

    [HttpPost(Name = "CreateSubscription")]
    public async Task<ActionResult<SubscriptionDto>> Create([FromBody] SubscriptionCreateDto subscriptionDto)
    {
        var result = await _subscriptionService.CreateAsync(subscriptionDto);

        if (result.IsError)
        {
            return result.ToActionResult();
        }

        return CreatedAtRoute("GetSubscriptionById", new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:int}", Name = "UpdateSubscription")]
    public async Task<ActionResult<SubscriptionDto>> Update(int id, [FromBody] SubscriptionUpdateDto subscriptionDto)
    {
        subscriptionDto.Id = id;
        var result = await _subscriptionService.UpdateAsync(subscriptionDto);

        if (result.IsError)
        {
            return result.ToActionResult();
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:int}", Name = "DeleteSubscription")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _subscriptionService.DeleteAsync(id);

        return result.ToActionResult();
    }

    [HttpGet("subscribe/{creatorId}", Name = "GetSubscribeInfo")]
    public async Task<ActionResult<object>> GetSubscribeInfo(string creatorId)
    {
        if (string.IsNullOrEmpty(creatorId))
        {
            return BadRequest(new { error = "Creator ID is required." });
        }

        var currentUserId = _currentUserService.GetUserId();
        if (currentUserId is null)
        {
            return Unauthorized(new { error = "User not authenticated." });
        }

        if (currentUserId == creatorId)
        {
            return BadRequest(new { error = "You cannot subscribe to yourself." });
        }

        var creator = await _userService.GetByIdAsync(creatorId);
        if (creator is null)
        {
            return NotFound(new { error = "Creator not found." });
        }

        return Ok(new
        {
            creatorId = creatorId,
            creatorName = creator.UserName,
            monthlyPrice = creator.PricePerMonth ?? 0
        });
    }

    [HttpPost("subscribe", Name = "ProcessSubscription")]
    public async Task<ActionResult<PaymentResultDto>> ProcessSubscription([FromBody] SubscribeRequestDto request)
    {
        var currentUserId = _currentUserService.GetUserId();
        if (currentUserId is null)
        {
            return Unauthorized(new { error = "User not authenticated." });
        }

        if (string.IsNullOrEmpty(request.CreatorId))
        {
            return BadRequest(new { error = "Creator ID is required." });
        }

        if (currentUserId == request.CreatorId)
        {
            return BadRequest(new { error = "You cannot subscribe to yourself." });
        }

        var paymentDto = new ProcessPaymentDto
        {
            OrdererId = currentUserId,
            CreatorId = request.CreatorId,
            Timeframe = request.Timeframe,
            GiftCardCode = request.GiftCardCode
        };

        var result = await _paymentService.ProcessSubscriptionPaymentAsync(paymentDto);

        if (result.IsError)
        {
            var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Payment failed for user {UserId} subscribing to {CreatorId}: {Error}",
                currentUserId, request.CreatorId, errorMessage);
            
            return result.ToActionResult();
        }

        var paymentResult = result.Value;
        _logger.LogInformation("User {UserId} successfully subscribed to {CreatorId} for {Amount}",
            currentUserId, request.CreatorId, paymentResult.FinalAmount);

        return Ok(paymentResult);
    }

    [HttpPost("validate-gift-card", Name = "ValidateGiftCard")]
    public async Task<ActionResult<object>> ValidateGiftCard([FromBody] ValidateGiftCardRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request?.Code))
        {
            return BadRequest(new { valid = false, message = "Please enter a gift card code." });
        }

        var result = await _paymentService.ValidateGiftCardCodeAsync(request.Code);

        if (result.IsError)
        {
            var firstError = result.Errors.FirstOrDefault();
            var errorMessage = firstError.Description ?? "Invalid gift card code.";
            return Ok(new { valid = false, message = errorMessage });
        }

        return Ok(new { valid = true, discount = result.Value, message = $"Gift card valid! Discount: ${result.Value:F2}" });
    }
}
