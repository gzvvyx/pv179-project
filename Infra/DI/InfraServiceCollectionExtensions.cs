using Infra.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.DI;

public static class InfraServiceCollectionExtensions
{
    public static IServiceCollection AddInfraServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVideoRepository, VideoRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IPlaylistRepository, PlaylistRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IGiftCardRepository, GiftCardRepository>();
        services.AddScoped<IGiftCardCodeRepository, GiftCardCodeRepository>();

        return services;
    }
}
