using ApplicationService.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Features.Common
{
    public class CachingBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ICacheService _cache;

        public CachingBehavior(ICacheService cache)
        {
            _cache = cache;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            // Sadece cachelenebilir request’ler için çalışsın
            if (request is not ICacheableRequest cacheableRequest)
                return await next();

            string cacheKey = cacheableRequest.CacheKey;
            var cachedData = await _cache.GetAsync<TResponse>(cacheKey);

            if (cachedData != null)
            {
                return cachedData;
            }

            var response = await next();

            await _cache.SetAsync(
                cacheKey,
                response,
                absoluteExpireTime: cacheableRequest.AbsoluteExpirationRelativeToNow
            );

            return response;
        }
    }

}
