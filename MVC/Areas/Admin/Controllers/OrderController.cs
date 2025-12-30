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
public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IEditOrderViewModelFactory _viewModelFactory;
    private readonly ILogger<OrderController> _logger;

    public OrderController(
        IOrderService orderService,
        IEditOrderViewModelFactory viewModelFactory,
        ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _viewModelFactory = viewModelFactory;
        _logger = logger;
    }

    [Route("", Name = "AdminOrders")]
    public async Task<IActionResult> Index()
    {
        var orders = await _orderService.GetAllAsync();
        var viewModels = orders.Select(o => new OrderIndexViewModel
        {
            Id = o.Id,
            OrdererUserName = o.Orderer.UserName,
            OrdererId = o.Orderer.Id,
            CreatorUserName = o.Creator.UserName,
            CreatorId = o.Creator.Id,
            Amount = o.Amount,
            Status = o.Status,
            CreatedAt = o.CreatedAt
        }).ToList();
        return View(viewModels);
    }

    [Route("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        var model = await _viewModelFactory.CreateAsync(order);
        return View(model);
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditOrderViewModel model)
    {
        var order = await _orderService.GetByIdAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        var orderUpdateDto = new OrderUpdateDto
        {
            Id = id,
            Amount = model.Amount,
            Status = model.Status
        };

        var result = await _orderService.UpdateAsync(orderUpdateDto);
        
        if (result.IsError)
        {
            result.AddErrorsToModelState(ModelState);
            
            await _viewModelFactory.PopulateViewModelAsync(model, order);
            return View(model);
        }
        
        return RedirectToAction(nameof(Index));
    }
}

