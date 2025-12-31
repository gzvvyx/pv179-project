namespace Infra.Services;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key) where T : class;
    Task SetAsync<T>(string key, T value) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null, TimeSpan? slidingExpiration = null) where T : class;
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> fetchData) where T : class;
    Task RemoveAsync(string key);
    void Remove(string key);
}

