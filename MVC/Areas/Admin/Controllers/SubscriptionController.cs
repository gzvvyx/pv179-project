using Business.DTOs;
using Business.Services;
using DAL.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Areas.Admin.Factories;
using MVC.Areas.Admin.Models;
using MVC.Extensions;

namespace MVC.Areas.Admin.Controllers;

[Area("Admin")]
[Route("[area]/[controller]")]
[Authorize(Roles = AppRoles.Admin)]
public class SubscriptionController : Controller
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IEditSubscriptionViewModelFactory _viewModelFactory;
    private readonly ILogger<SubscriptionController> _logger;

    public SubscriptionController(
        ISubscriptionService subscriptionService,
        IEditSubscriptionViewModelFactory viewModelFactory,
        ILogger<SubscriptionController> logger)
    {
        _subscriptionService = subscriptionService;
        _viewModelFactory = viewModelFactory;
        _logger = logger;
    }

    [Route("", Name = "AdminSubscriptions")]
    public async Task<IActionResult> Index()
    {
        var subscriptions = await _subscriptionService.GetAllAsync();
        var viewModels = subscriptions.Select(s => new SubscriptionIndexViewModel
        {
            Id = s.Id,
            OrdererUserName = s.Orderer.UserName,
            OrdererId = s.Orderer.Id,
            CreatorUserName = s.Creator.UserName,
            CreatorId = s.Creator.Id,
            Active = s.Active,
            Timeframe = s.Timeframe,
            SubscribedAt = s.SubscribedAt,
            ExpiresAt = s.ExpiresAt
        }).ToList();
        return View(viewModels);
    }

    [Route("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var subscription = await _subscriptionService.GetByIdAsync(id);
        if (subscription == null)
        {
            return NotFound();
        }

        var model = await _viewModelFactory.CreateAsync(subscription);
        return View(model);
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditSubscriptionViewModel model)
    {
        var subscription = await _subscriptionService.GetByIdAsync(id);
        if (subscription == null)
        {
            return NotFound();
        }

        var subscriptionUpdateDto = new SubscriptionUpdateDto
        {
            Id = id,
            Active = model.Active,
            Timeframe = model.Timeframe,
            LastRenewedAt = model.LastRenewedAt,
            ExpiresAt = model.ExpiresAt
        };

        var result = await _subscriptionService.UpdateAsync(subscriptionUpdateDto);
        
        if (result.IsError)
        {
            result.AddErrorsToModelState(ModelState);
            
            await _viewModelFactory.PopulateViewModelAsync(model, subscription);
            return View(model);
        }
        
        return RedirectToAction(nameof(Index));
    }
}

