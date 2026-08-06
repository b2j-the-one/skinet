namespace Core.Interfaces;

public interface ICacheService
{
    Task SetAsync(string key, object value);

    Task<T?> GetAsync<T>(string key);

    Task RemoveAsync(string key);
}