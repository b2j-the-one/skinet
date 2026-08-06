using Core.Interfaces;

namespace Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    public Task<T?> GetAsync<T>(string key)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task SetAsync(string key, object value)
    {
        throw new NotImplementedException();
    }
}