using Microsoft.Extensions.Caching.Memory;

namespace Infra.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    private static readonly TimeSpan DefaultAbsoluteExpiration = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan DefaultSlidingExpiration = TimeSpan.FromMinutes(2);

    public Task<T?> GetAsync<T>(string key) where T : class
    {
        _memoryCache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value) where T : class
    {
        return SetAsync(key, value, DefaultAbsoluteExpiration, DefaultSlidingExpiration);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null, TimeSpan? slidingExpiration = null) where T : class
    {
        var cacheOptions = new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.Normal
        };

        if (absoluteExpirationRelativeToNow.HasValue)
        {
            cacheOptions.AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow.Value;
        }

        if (slidingExpiration.HasValue)
        {
            cacheOptions.SlidingExpiration = slidingExpiration.Value;
        }

        _memoryCache.Set(key, value, cacheOptions);
        return Task.CompletedTask;
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> fetchData) where T : class
    {
        var cachedValue = await GetAsync<T>(key);
        if (cachedValue != null)
        {
            return cachedValue;
        }

        var data = await fetchData();
        await SetAsync(key, data);
        return data;
    }

    public Task RemoveAsync(string key)
    {
        _memoryCache.Remove(key);
        return Task.CompletedTask;
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }
}

