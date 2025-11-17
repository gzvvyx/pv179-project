using DAL.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Common.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        return user.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
