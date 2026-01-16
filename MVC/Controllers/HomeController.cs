using System.Diagnostics;
using System.Security.Claims;
using Business.Services;
using Infra.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pv179.Models;

namespace pv179.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IVideoService _videoService;

    public HomeController(
        ILogger<HomeController> logger,
        ISubscriptionService subscriptionService,
        IVideoService videoService)
    {
        _logger = logger;
        _subscriptionService = subscriptionService;
        _videoService = videoService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return View(new HomeFeedViewModel { HasSubscriptions = false });
        }

        var subscriptionsResult = await _subscriptionService.GetBySubscriberIdAsync(userId);
        
        if (subscriptionsResult.IsError || !subscriptionsResult.Value.Any())
        {
            return View(new HomeFeedViewModel { HasSubscriptions = false });
        }

        var activeSubscriptions = subscriptionsResult.Value
            .Where(s => s.Active && s.ExpiresAt > DateTime.UtcNow)
            .ToList();

        if (!activeSubscriptions.Any())
        {
            return View(new HomeFeedViewModel { HasSubscriptions = false });
        }

        var creatorIds = activeSubscriptions.Select(s => s.Creator.Id).ToList();

        var videos = await _videoService.GetByFilterAsync(new VideoFilterDto
        {
            CreatorIds = creatorIds,
            SortBy = "CreatedAt",
            SortDescending = true,
            PageSize = 50
        });

        return View(new HomeFeedViewModel
        {
            Videos = videos,
            HasSubscriptions = true
        });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}