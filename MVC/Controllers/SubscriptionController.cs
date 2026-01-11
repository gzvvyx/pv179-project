using Business.DTOs;
using Business.Services;
using DAL.Models.Enums;
using DAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pv179.Models;

namespace pv179.Controllers;

[Authorize]
public class SubscriptionController : Controller
{
    private readonly ILogger<SubscriptionController> _logger;
    private readonly IPaymentService _paymentService;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUserService;

    public SubscriptionController(
        ILogger<SubscriptionController> logger,
        IPaymentService paymentService,
        IUserService userService,
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _paymentService = paymentService;
        _userService = userService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Displays the subscription payment page for a creator
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Subscribe(string creatorId)
    {
        if (string.IsNullOrEmpty(creatorId))
        {
            return BadRequest("Creator ID is required.");
        }

        var currentUserId = _currentUserService.GetUserId();
        if (currentUserId is null)
        {
            return Unauthorized();
        }

        if (currentUserId == creatorId)
        {
            TempData["Error"] = "You cannot subscribe to yourself.";
            return RedirectToAction("Index", "User", new { id = creatorId });
        }

        var creator = await _userService.GetByIdAsync(creatorId);
        if (creator is null)
        {
            return NotFound("Creator not found.");
        }

        var viewModel = new SubscribeViewModel
        {
            CreatorId = creatorId,
            CreatorName = creator.UserName,
            MonthlyPrice = creator.PricePerMonth ?? 0,
            Timeframe = SubscriptionTimeframe.Month
        };

        return View(viewModel);
    }

    /// <summary>
    /// Processes the subscription payment
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Subscribe(SubscribeViewModel model)
    {
        var currentUserId = _currentUserService.GetUserId();
        if (currentUserId is null)
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            // Reload creator info
            var creator = await _userService.GetByIdAsync(model.CreatorId);
            if (creator is not null)
            {
                model.CreatorName = creator.UserName;
                model.MonthlyPrice = creator.PricePerMonth ?? 0;
            }
            return View(model);
        }

        var paymentDto = new ProcessPaymentDto
        {
            OrdererId = currentUserId,
            CreatorId = model.CreatorId,
            Timeframe = model.Timeframe,
            GiftCardCode = model.GiftCardCode
        };

        var result = await _paymentService.ProcessSubscriptionPaymentAsync(paymentDto);

        if (result.IsError)
        {
            var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Payment failed for user {UserId} subscribing to {CreatorId}: {Error}", 
                currentUserId, model.CreatorId, errorMessage);
            
            TempData["Error"] = errorMessage;
            
            // Reload creator info
            var creator = await _userService.GetByIdAsync(model.CreatorId);
            if (creator is not null)
            {
                model.CreatorName = creator.UserName;
                model.MonthlyPrice = creator.PricePerMonth ?? 0;
            }
            return View(model);
        }

        var paymentResult = result.Value;
        var resultViewModel = new SubscribeResultViewModel
        {
            Success = paymentResult.Success,
            CreatorName = model.CreatorName,
            OriginalAmount = paymentResult.OriginalAmount,
            FinalAmount = paymentResult.FinalAmount,
            DiscountApplied = paymentResult.DiscountApplied,
            GiftCardCodeUsed = paymentResult.GiftCardCodeUsed,
            ExpiresAt = paymentResult.Subscription?.ExpiresAt
        };

        _logger.LogInformation("User {UserId} successfully subscribed to {CreatorId} for {Amount}", 
            currentUserId, model.CreatorId, paymentResult.FinalAmount);

        return View("SubscribeResult", resultViewModel);
    }

    /// <summary>
    /// AJAX endpoint to validate gift card code
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> ValidateGiftCard([FromBody] ValidateGiftCardRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Code))
        {
            return Json(new { valid = false, message = "Please enter a gift card code." });
        }

        var result = await _paymentService.ValidateGiftCardCodeAsync(request.Code);

        if (result.IsError)
        {
            var firstError = result.Errors.FirstOrDefault();
            var errorMessage = firstError.Description ?? "Invalid gift card code.";
            return Json(new { valid = false, message = errorMessage });
        }

        return Json(new { valid = true, discount = result.Value, message = $"Gift card valid! Discount: ${result.Value:F2}" });
    }
}

public class ValidateGiftCardRequest
{
    public string? Code { get; set; }
}

