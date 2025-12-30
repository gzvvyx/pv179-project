using Business.DTOs;
using Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GiftCardCodeController : ControllerBase
{
    private readonly ILogger<GiftCardCodeController> _logger;
    private readonly IGiftCardCodeService _giftCardCodeService;

    public GiftCardCodeController(ILogger<GiftCardCodeController> logger, IGiftCardCodeService giftCardCodeService)
    {
        _logger = logger;
        _giftCardCodeService = giftCardCodeService;
    }

    [HttpGet(Name = "GetGiftCardCodes")]
    public async Task<IEnumerable<GiftCardCodeDto>> Get()
    {
        return await _giftCardCodeService.GetAllAsync();
    }

    [HttpGet("{id:int}", Name = "GetGiftCardCodeById")]
    public async Task<ActionResult<GiftCardCodeDto>> GetByCode(string code)
    {
        var giftCardCode = await _giftCardCodeService.GetByCodeAsync(code);
        if (giftCardCode is null)
        {
            return NotFound();
        }
        return Ok(giftCardCode);
    }

    [HttpPost(Name = "CreateGiftCardCode")]
    public async Task<ActionResult<GiftCardCodeDto>> Create([FromBody] GiftCardCodeCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (result, giftCardCode) = await _giftCardCodeService.CreateAsync(dto);

        if (!result.Succeeded || giftCardCode is null)
        {
            return BadRequest(result.Errors.Select(error => error.Description));
        }

        return CreatedAtRoute("GetGiftCardCodeById", new { code = giftCardCode.Code }, giftCardCode);
    }

    [HttpPut("{code}", Name = "UpdateGiftCardCode")]
    public async Task<ActionResult<GiftCardCodeDto>> Update(string code, [FromBody] GiftCardCodeUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var (result, giftCardCode) = await _giftCardCodeService.UpdateAsync(code, dto);

        if (result.Succeeded || giftCardCode is not null)
        {
            return Ok(giftCardCode);
        }

        if (result.Errors.Any(error => error.Code == "GiftCardCodeNotFound"))
        {
            return NotFound();
        }

        return BadRequest(result.Errors.Select(error => error.Description));
    }

    [HttpDelete("{code}", Name = "DeleteGiftCardCode")]
    public async Task<IActionResult> Delete(string code)
    {
        var result = await _giftCardCodeService.DeleteAsync(code);

        if (result.Succeeded)
        {
            return NoContent();
        }

        if (result.Errors.Any(error => error.Code == "GiftCardCodeNotFound"))
        {
            return NotFound();
        }

        return BadRequest(result.Errors.Select(error => error.Description));
    }
}