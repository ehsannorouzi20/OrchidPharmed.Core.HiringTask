using Microsoft.Extensions.Caching.Distributed;
using OrchidPharmed.Core.HiringTask.API.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OrchidPharmed.Core.HiringTask.API.Structure
{
    public interface ICacheManager
    {
        Task SetCacheAsync(string key, object data);
        Task<T> GetCacheAsync<T>(string key, Func<Task<T>> loadfunctionAsync);
        Task RemoveAsync(string key);
    }
    public class CacheManager : ICacheManager
    {
        private readonly IDistributedCache _cache;
        private readonly SemaphoreSlim _cachedAsyncLock;
        public CacheManager(IDistributedCache cache)
        {
            _cache = cache;
            _cachedAsyncLock = new SemaphoreSlim(1, 1);
        }
        public async Task<T> GetCacheAsync<T>(string key, Func<Task<T>> loadfunction)
        {
            var result = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(result))
            {
                await _cachedAsyncLock.WaitAsync();
                try
                {
                    result = await _cache.GetStringAsync(key);
                    if (!string.IsNullOrEmpty(result))
                        return System.Text.Json.JsonSerializer.Deserialize<T>(result);
                    var res = await loadfunction();
                    if (res == null)
                        return default(T);
                    await _cache.SetStringAsync(key, System.Text.Json.JsonSerializer.Serialize(res));
                    return (T)res;
                }
                finally
                {
                    _cachedAsyncLock.Release();
                }
            }
            return System.Text.Json.JsonSerializer.Deserialize<T>(result);
        }

        public async Task RemoveAsync(string key)
        {
            await _cachedAsyncLock.WaitAsync();
            try
            {
                await _cache.RemoveAsync(key);
            }
            finally
            {
                _cachedAsyncLock.Release();
            }
        }

        public async Task SetCacheAsync(string key, object data)
        {
            await _cachedAsyncLock.WaitAsync();
            try
            {
                await _cache.RemoveAsync(key);
                await _cache.SetStringAsync(key, System.Text.Json.JsonSerializer.Serialize(data));
            }
            finally
            {
                _cachedAsyncLock.Release();
            }
        }
    }
}
