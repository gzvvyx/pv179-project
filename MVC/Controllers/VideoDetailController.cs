using Microsoft.AspNetCore.Mvc;
using Business.Services;
using System.Threading.Tasks;

namespace MVC.Controllers;

public class VideoDetailController : Controller
{
    private readonly IVideoService _videoService;

    public VideoDetailController(IVideoService videoService)
    {
        _videoService = videoService;
    }

    public async Task<IActionResult> Detail(int id)
    {
        var video = await _videoService.GetByIdAsync(id);
        if (video == null)
            return NotFound();
        return View(video);
    }
}
