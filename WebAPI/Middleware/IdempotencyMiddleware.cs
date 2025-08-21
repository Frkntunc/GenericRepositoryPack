using Microsoft.Extensions.Caching.Distributed;
using WebAPI.Filters;

namespace WebAPI.Middleware
{
    public class IdempotencyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _cache;
        private readonly ILogger<IdempotencyMiddleware> _logger;

        public IdempotencyMiddleware(RequestDelegate next, IDistributedCache cache, ILogger<IdempotencyMiddleware> logger)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var attr = endpoint?.Metadata.GetMetadata<IdempotentAttribute>();

            if (attr == null)
            {
                // Attribute yok → normal çalış
                await _next(context);
                return;
            }

            // Header var mı?
            if (!context.Request.Headers.TryGetValue("Idempotency-Key", out var key))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Missing Idempotency-Key header.");
                return;
            }

            var cacheKey = $"idempotency:{key}";

            // Daha önce aynı key kullanılmış mı?
            var cachedResponse = await _cache.GetStringAsync(cacheKey);
            if (cachedResponse != null)
            {
                _logger.LogInformation("Returning cached response for key {Key}", key);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(cachedResponse);
                return;
            }

            // Response’u yakalamak için buffer kullanıyoruz
            var originalBody = context.Response.Body;
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            await _next(context); // pipeline devam etsin

            // Response'u cache'e kaydet
            memoryStream.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(memoryStream).ReadToEndAsync();

            await _cache.SetStringAsync(
                cacheKey,
                responseText,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(attr.ExpirationSeconds)
                });

            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(originalBody);
            context.Response.Body = originalBody;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class IdempotentAttribute : Attribute
    {
        public int ExpirationSeconds { get; }

        public IdempotentAttribute(int expirationSeconds)
        {
            ExpirationSeconds = expirationSeconds;
        }
    }
}
