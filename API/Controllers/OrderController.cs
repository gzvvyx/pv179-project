using Business.DTOs;
using Business.Services;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;
    private readonly IOrderService _orderService;

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

        return await _orderService.GetByOrdererIdAsync(userId);
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

    [HttpPost(Name = "CreateOrder")]
    public async Task<ActionResult<OrderDto>> Create([FromBody] OrderCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (result, order) = await _orderService.CreateAsync(dto);

        if (!result.Succeeded || order is null)
        {
            return BadRequest(result.Errors.Select(error => error.Description));
        }

        return CreatedAtRoute("GetOrderById", new { id = order.Id }, order);
    }

    [HttpPut("{id:int}", Name = "UpdateOrder")]
    public async Task<ActionResult<OrderDto>> Update(int id, [FromBody] OrderUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (result, order) = await _orderService.UpdateAsync(id, dto);

        if (result.Succeeded || order is not null)
        {
            return Ok(order);
        }

        if (result.Errors.Any(error => error.Code == "OrderNotFound"))
        {
            return NotFound();
        }

        return BadRequest(result.Errors.Select(error => error.Description));
    }

    [HttpDelete("{id:int}", Name = "DeleteOrder")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _orderService.DeleteAsync(id);

        if (result.Succeeded)
        {
            return NoContent();
        }

        if (result.Errors.Any(error => error.Code == "OrderNotFound"))
        {
            return NotFound();
        }

        return BadRequest(result.Errors.Select(error => error.Description));
    }
}
