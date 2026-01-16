using API.Extensions;
using API.DTOs;
using API.Mappers;
using Business.DTOs;
using Business.Mappers;
using Business.Services;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly IOrderService _orderService;
    private readonly OrderRequestMapper _requestMapper = new();
    private readonly OrderResponseMapper _responseMapper = new();

    public OrderController(ILogger<OrderController> logger, IOrderService orderService)
    {
        _logger = logger;
        _orderService = orderService;
    }

    [HttpGet(Name = "GetOrders")]
    public async Task<IEnumerable<OrderDto>> Get()
    {
        return await _orderService.GetAllAsync();
    }

    [HttpGet("my-orders", Name = "GetMyOrders")]
    public async Task<IEnumerable<OrderDto>> GetMyOrders()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userId))
        {
            return Enumerable.Empty<OrderDto>();
        }

        var orders = await _orderService.GetByOrdererIdAsync(userId);
        if (orders.IsError)
        {
            return Enumerable.Empty<OrderDto>();
        }
        return orders.Value;
    }

    [HttpGet("with-users", Name = "GetOrdersWithUsers")]
    public async Task<IEnumerable<OrderWithUsersDto>> GetAllWithUsers()
    {
        return await _orderService.GetAllWithUsersAsync();
    }

    [HttpGet("{id:int}", Name = "GetOrderById")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await _orderService.GetByIdAsync(id);

        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpGet("{id:int}/details", Name = "GetOrderDetailsById")]
    public async Task<ActionResult<OrderWithUsersDto>> GetDetails(int id)
    {
        var order = await _orderService.GetByIdAsync(id);

        if (order is null)
        {
            return NotFound();
        }

        var orderWithUsers = _responseMapper.ToOrderWithUsersDto(order);
        return Ok(orderWithUsers);
    }

    [HttpPost(Name = "CreateOrder")]
    public async Task<ActionResult<OrderDto>> Create([FromBody] OrderCreateRequestDto dto)
    {
        var createDto = _requestMapper.ToBusinessDto(dto);
        var result = await _orderService.CreateAsync(createDto);

        if (result.IsError)
        {
            return result.ToActionResult();
        }

        return CreatedAtRoute("GetOrderById", new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:int}", Name = "UpdateOrder")]
    public async Task<ActionResult<OrderDto>> Update(int id, [FromBody] OrderUpdateRequestDto dto)
    {
        var updateDto = _requestMapper.ToBusinessDto(dto, id);
        var result = await _orderService.UpdateAsync(updateDto);

        return result.ToActionResult();
    }

    [HttpDelete("{id:int}", Name = "DeleteOrder")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _orderService.DeleteAsync(id);

        return result.ToActionResult();
    }
}
