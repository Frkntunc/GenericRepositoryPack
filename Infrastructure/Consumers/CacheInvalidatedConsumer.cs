using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using Shared.Events;

namespace Infrastructure.Consumers
{
    //Consumerlar ayrı bir Worker Projeye taşınabilir. Mikroservice için daha uyumlu
    public class CacheInvalidatedConsumer : IConsumer<CacheInvalidatedEvent>
    {
        private readonly IDistributedCache _cache;

        public CacheInvalidatedConsumer(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task Consume(ConsumeContext<CacheInvalidatedEvent> context)
        {
            var cacheKey = context.Message.CacheKey;
            await _cache.RemoveAsync(cacheKey);
        }
    }

}
