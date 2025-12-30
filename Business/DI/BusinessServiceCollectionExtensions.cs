using Business.DTOs;
using Business.Services;
using Business.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Business.DI;

public static class BusinessServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IVideoService, VideoService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IPlaylistService, PlaylistService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IGiftCardService, GiftCardService>();
        services.AddScoped<IGiftCardCodeService, GiftCardCodeService>();

        // Register validators
        services.AddScoped<IValidator<UserCreateDto>, UserCreateDtoValidator>();
        services.AddScoped<IValidator<UserUpdateDto>, UserUpdateDtoValidator>();
        services.AddScoped<IValidator<OrderCreateDto>, OrderCreateDtoValidator>();
        services.AddScoped<IValidator<OrderUpdateDto>, OrderUpdateDtoValidator>();
        services.AddScoped<IValidator<PlaylistCreateDto>, PlaylistCreateDtoValidator>();
        services.AddScoped<IValidator<PlaylistUpdateDto>, PlaylistUpdateDtoValidator>();
        services.AddScoped<IValidator<SubscriptionCreateDto>, SubscriptionCreateDtoValidator>();
        services.AddScoped<IValidator<SubscriptionUpdateDto>, SubscriptionUpdateDtoValidator>();
        services.AddScoped<IValidator<CommentCreateDto>, CommentCreateDtoValidator>();
        services.AddScoped<IValidator<CommentUpdateDto>, CommentUpdateDtoValidator>();
        services.AddScoped<IValidator<VideoCreateDto>, VideoCreateDtoValidator>();
        services.AddScoped<IValidator<VideoUpdateDto>, VideoUpdateDtoValidator>();
        services.AddScoped<IValidator<GiftCardCreateDto>, GiftCardCreateDtoValidator>();
        services.AddScoped<IValidator<GiftCardUpdateDto>, GiftCardUpdateDtoValidator>();
        services.AddScoped<IValidator<GiftCardCodeCreateDto>, GiftCardCodeCreateDtoValidator>();
        services.AddScoped<IValidator<GiftCardCodeUpdateDto>, GiftCardCodeUpdateDtoValidator>();
        services.AddScoped<IValidator<CategoryCreateDto>, CategoryCreateDtoValidator>();
        services.AddScoped<IValidator<CategoryUpdateDto>, CategoryUpdateDtoValidator>();

        return services;
    }
}
