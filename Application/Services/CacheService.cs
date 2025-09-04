using ApplicationService.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApplicationService.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var cachedData = await _cache.GetStringAsync(key);
            if (cachedData == null)
                return default;

            return JsonSerializer.Deserialize<T>(cachedData);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions();

            if (absoluteExpireTime.HasValue)
                options.AbsoluteExpirationRelativeToNow = absoluteExpireTime;

            if (slidingExpireTime.HasValue)
                options.SlidingExpiration = slidingExpireTime;

            var jsonData = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, jsonData, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
