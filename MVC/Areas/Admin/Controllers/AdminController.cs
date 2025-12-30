using DAL.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Areas.Admin.Controllers;

[Area("Admin")]
[Route("[area]")]
[Authorize(Roles = AppRoles.Admin)]
public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;

    public AdminController(ILogger<AdminController> logger)
    {
        _logger = logger;
    }

    [Route("", Name = "AdminIndex")]
    public IActionResult Index()
    {
        return View();
    }
}

