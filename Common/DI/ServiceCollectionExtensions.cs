using Common.Services;
using DAL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Common.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCurrentUser(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }
}
