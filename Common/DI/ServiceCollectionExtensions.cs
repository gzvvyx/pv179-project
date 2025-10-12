using Business.Services;
using Infra.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Common.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        // Repositories 
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVideoRepository, VideoRepository>();
        
        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IVideoService, VideoService>();
        
        return services;
    }
}
