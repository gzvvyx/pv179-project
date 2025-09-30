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
        
        // Services
        services.AddScoped<IUserService, UserService>();
        
        return services;
    }
}