using Business.Services;
using DAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pv179.Mappers;

namespace pv179.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly ILogger<OrderController> _logger;
    private readonly IOrderService _orderService;
    private readonly ICurrentUserService _currentUserService;
    private readonly OrderViewMapper _mapper = new();

    public OrderController(
        ILogger<OrderController> logger, 
        IOrderService orderService,
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _orderService = orderService;
        _currentUserService = currentUserService;
    }

    public async Task<IActionResult> MyOrders()
    {
        var userId = _currentUserService.GetUserId() 
            ?? throw new UnauthorizedAccessException("User ID not found in authenticated context.");

        var orders = await _orderService.GetByOrdererIdAsync(userId);
        var viewModel = _mapper.MapToViewModelList(orders);

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = _currentUserService.GetUserId() 
            ?? throw new UnauthorizedAccessException("User ID not found in authenticated context.");

        var order = await _orderService.GetByIdAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        if (order.Orderer.Id != userId)
        {
            return Forbid();
        }

        var viewModel = _mapper.MapToDetailsViewModel(order);

        return View(viewModel);
    }
}
