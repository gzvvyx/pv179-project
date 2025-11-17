using Business.DTOs;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionController : ControllerBase
{

    private readonly ILogger<SubscriptionController> _logger;
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionController(ILogger<SubscriptionController> logger, ISubscriptionService subscriptionService)
    {
        _logger = logger;
        _subscriptionService = subscriptionService;
    }

    [HttpGet(Name = "GetSubscriptions")]
    public async Task<IEnumerable<SubscriptionDto>> Get()
    {
        return await _subscriptionService.GetAllAsync();
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
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var (result, subscription) = await _subscriptionService.CreateAsync(subscriptionDto);

        if (!result.Succeeded || subscription is null)
        {
            return BadRequest(result.Errors.Select(error => error.Description));
        }

        return CreatedAtRoute("GetSubscriptionById", new { id = subscription.Id }, subscription);
    }

    [HttpPut("{id:int}", Name = "UpdateSubscription")]
    public async Task<ActionResult<SubscriptionDto>> Update(int id, [FromBody] SubscriptionUpdateDto subscriptionDto)
    {
        if (!ModelState.IsValid)
        {
            return new BadRequestObjectResult(ModelState);
        }

        var (result, subscription) = await _subscriptionService.UpdateAsync(id, subscriptionDto);

        if (result.Succeeded || subscription is not null)
        {
            return Ok(subscription);
        }

        if (result.Errors.Any(e => e.Code == "SubscriptionNotFound"))
        {
            return NotFound();
        }

        return BadRequest(result.Errors.Select(error => error.Description));
    }

    [HttpDelete("{id:int}", Name = "DeleteSubscription")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _subscriptionService.DeleteAsync(id);

        if (result.Succeeded)
        {
            return NoContent();
        }

        if (result.Errors.Any(e => e.Code == "SubscriptionNotFound"))
        {
            return NotFound();
        }

        return BadRequest(result.Errors.Select(error => error.Description));
    }
}
