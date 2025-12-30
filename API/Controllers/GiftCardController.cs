using Business.DTOs;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GiftCardController : ControllerBase
{
    private readonly ILogger<GiftCardController> _logger;
    private readonly IGiftCardService _GiftCardService;

    public GiftCardController(ILogger<GiftCardController> logger, IGiftCardService GiftCardService)
    {
        _logger = logger;
        _GiftCardService = GiftCardService;
    }

    [HttpGet(Name = "GetGiftCards")]
    public async Task<IEnumerable<GiftCardDto>> Get()
    {
        return await _GiftCardService.GetAllAsync();
    }

    [HttpGet("{id:int}", Name = "GetGiftCardById")]
    public async Task<ActionResult<GiftCardDto>> GetById(int id)
    {
        var GiftCard = await _GiftCardService.GetByIdAsync(id);

        if (GiftCard is null)
        {
            return NotFound();
        }

        return Ok(GiftCard);
    }

    [HttpPost(Name = "CreateGiftCard")]
    public async Task<ActionResult<GiftCardDto>> Create([FromBody] GiftCardCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (result, GiftCard) = await _GiftCardService.CreateAsync(dto);

        if (!result.Succeeded || GiftCard is null)
        {
            return BadRequest(result.Errors.Select(error => error.Description));
        }

        return CreatedAtRoute("GetGiftCardById", new { id = GiftCard.Id }, GiftCard);
    }

    [HttpPut("{id:int}", Name = "UpdateGiftCard")]
    public async Task<ActionResult<GiftCardDto>> Update(int id, [FromBody] GiftCardUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (result, GiftCard) = await _GiftCardService.UpdateAsync(id, dto);

        if (result.Succeeded || GiftCard is not null)
        {
            return Ok(GiftCard);
        }

        if (result.Errors.Any(error => error.Code == "GiftCardNotFound"))
        {
            return NotFound();
        }

        return BadRequest(result.Errors.Select(error => error.Description));
    }

    [HttpDelete("{id:int}", Name = "DeleteGiftCard")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _GiftCardService.DeleteAsync(id);

        if (result.Succeeded)
        {
            return NoContent();
        }

        if (result.Errors.Any(error => error.Code == "GiftCardNotFound"))
        {
            return NotFound();
        }

        return BadRequest(result.Errors.Select(error => error.Description));
    }
}