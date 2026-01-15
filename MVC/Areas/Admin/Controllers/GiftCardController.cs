using Business.DTOs;
using Business.Services;
using DAL.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC.Areas.Admin.Models;
using MVC.Extensions;

namespace MVC.Areas.Admin.Controllers;

[Area("Admin")]
[Route("[area]/[controller]")]
[Authorize(Roles = AppRoles.Admin)]
public class GiftCardController : Controller
{
    private readonly IGiftCardService _giftCardService;
    private readonly IGiftCardCodeService _giftCardCodeService;
    private readonly ILogger<GiftCardController> _logger;

    public GiftCardController(
        IGiftCardService giftCardService,
        IGiftCardCodeService giftCardCodeService,
        ILogger<GiftCardController> logger)
    {
        _giftCardService = giftCardService;
        _giftCardCodeService = giftCardCodeService;
        _logger = logger;
    }

    [Route("", Name = "AdminGiftCards")]
    public async Task<IActionResult> Index()
    {
        var giftCards = await _giftCardService.GetAllAsync();
        var viewModels = giftCards.Select(gc => new GiftCardIndexViewModel
        {
            Id = gc.Id,
            PriceReduction = gc.PriceReduction,
            ValidFrom = gc.ValidFrom,
            ValidTo = gc.ValidTo,
            CodesCount = gc.Codes.Count,
            CreatedAt = gc.CreatedAt
        }).ToList();
        return View(viewModels);
    }

    [Route("Create")]
    public IActionResult Create()
    {
        return View(new CreateGiftCardViewModel());
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateGiftCardViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var giftCardCreateDto = new GiftCardCreateDto
        {
            PriceReduction = model.PriceReduction,
            ValidFrom = model.ValidFrom,
            ValidTo = model.ValidTo,
            GiftCardCodes = new List<GiftCardCodeDto>()
        };

        var result = await _giftCardService.CreateAsync(giftCardCreateDto);

        if (result.IsError)
        {
            result.AddErrorsToModelState(ModelState);
            return View(model);
        }

        TempData["SuccessMessage"] = "Gift card created successfully!";
        return RedirectToAction(nameof(Index));
    }

    [Route("Edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var giftCard = await _giftCardService.GetByIdAsync(id);
        if (giftCard == null)
        {
            return NotFound();
        }

        var model = new EditGiftCardViewModel
        {
            Id = giftCard.Id,
            PriceReduction = giftCard.PriceReduction,
            ValidFrom = giftCard.ValidFrom,
            ValidTo = giftCard.ValidTo,
            Codes = giftCard.Codes.ToList(),
            CreatedAt = giftCard.CreatedAt,
            UpdatedAt = giftCard.UpdatedAt
        };

        return View(model);
    }

    [HttpPost("Edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditGiftCardViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var giftCard = await _giftCardService.GetByIdAsync(id);
            if (giftCard == null)
            {
                return NotFound();
            }
            model.Codes = giftCard.Codes.ToList();
            return View(model);
        }

        var giftCardUpdateDto = new GiftCardUpdateDto
        {
            Id = id,
            PriceReduction = model.PriceReduction,
            GiftCardCodes = model.Codes
        };

        var result = await _giftCardService.UpdateAsync(giftCardUpdateDto);

        if (result.IsError)
        {
            result.AddErrorsToModelState(ModelState);
            var giftCard = await _giftCardService.GetByIdAsync(id);
            if (giftCard != null)
            {
                model.Codes = giftCard.Codes.ToList();
            }
            return View(model);
        }

        TempData["SuccessMessage"] = "Gift card updated successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _giftCardService.DeleteAsync(id);

        if (result.IsError)
        {
            TempData["ErrorMessage"] = "Failed to delete gift card.";
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = "Gift card deleted successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Codes/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCode(CreateGiftCardCodeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Invalid code data.";
            return RedirectToAction(nameof(Edit), new { id = model.GiftCardId });
        }

        var codeCreateDto = new GiftCardCodeCreateDto
        {
            Code = model.Code,
            Used = model.Used,
            GiftCardId = model.GiftCardId
        };

        var result = await _giftCardCodeService.CreateAsync(codeCreateDto);

        if (result.IsError)
        {
            TempData["ErrorMessage"] = "Failed to create gift card code.";
            return RedirectToAction(nameof(Edit), new { id = model.GiftCardId });
        }

        TempData["SuccessMessage"] = "Gift card code created successfully!";
        return RedirectToAction(nameof(Edit), new { id = model.GiftCardId });
    }

    [HttpPost("Codes/Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCode(string code, int giftCardId)
    {
        if (string.IsNullOrEmpty(code))
        {
            TempData["ErrorMessage"] = "Code is required.";
            return RedirectToAction(nameof(Edit), new { id = giftCardId });
        }

        var result = await _giftCardCodeService.DeleteAsync(code);

        if (result.IsError)
        {
            TempData["ErrorMessage"] = "Failed to delete gift card code.";
            return RedirectToAction(nameof(Edit), new { id = giftCardId });
        }

        TempData["SuccessMessage"] = "Gift card code deleted successfully!";
        return RedirectToAction(nameof(Edit), new { id = giftCardId });
    }
}

