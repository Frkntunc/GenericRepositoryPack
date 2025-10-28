using ApplicationService.Repositories;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services
{
    public class ConcurrencyService : IConcurrencyService
    {
        private readonly IDatabase _db;

        public ConcurrencyService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<bool> TryEnterAsync(string key, int maxConcurrent, TimeSpan ttl)
        {
            // increment active count
            var current = await _db.StringIncrementAsync(key);

            if (current == 1)
            {
                // ilk kez oluşturuluyorsa TTL ver
                await _db.KeyExpireAsync(key, ttl);
            }

            if (current > maxConcurrent)
            {
                // aşıldıysa geri al
                await _db.StringDecrementAsync(key);
                return false;
            }

            return true;
        }

        public async Task ExitAsync(string key)
        {
            // işlem tamamlandığında aktif istek sayısını azalt
            await _db.StringDecrementAsync(key);
        }
    }
}
