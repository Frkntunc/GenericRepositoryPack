using ApplicationService.Services.Common;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace ApplicationService.Services
{
    public class CacheService : ICacheService, ITransientService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var cachedData = await _cache.GetStringAsync(key, cancellationToken);
            if (cachedData == null)
                return default;

            return JsonSerializer.Deserialize<T>(cachedData);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpireTime = null, TimeSpan? slidingExpireTime = null, CancellationToken cancellationToken = default)
        {
            var options = new DistributedCacheEntryOptions();

            if (absoluteExpireTime.HasValue)
                options.AbsoluteExpirationRelativeToNow = absoluteExpireTime;

            if (slidingExpireTime.HasValue)
                options.SlidingExpiration = slidingExpireTime;

            var jsonData = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, jsonData, options, cancellationToken);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }
    }
}
