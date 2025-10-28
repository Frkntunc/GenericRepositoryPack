using ApplicationService.Repositories;
using ApplicationService.Services;
using Microsoft.Extensions.Options;
using Shared.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebAPI.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private readonly IRateLimitService _rateStore;
        private readonly IConcurrencyService _concurrencyStore;
        private readonly RateLimitingOptions _options;

        public RateLimitingMiddleware(
            RequestDelegate next,
            ILogger<RateLimitingMiddleware> logger,
            IRateLimitService rateStore,
            IConcurrencyService concurrencyStore,
            IOptions<RateLimitingOptions> options)
        {
            _next = next;
            _logger = logger;
            _rateStore = rateStore;
            _concurrencyStore = concurrencyStore;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var (key, limit, window, maxConcurrent, maxWait) = ResolveKeyAndLimits(context);
            var concurrentKey = $"{key}:concurrent";

            // Concurrency limit kontrolü (queued bekleme dahil)
            var entered = await _concurrencyStore.TryEnterAsync(concurrentKey, maxConcurrent, window);
            if (!entered)
            {
                _logger.LogWarning("Concurrency limit reached for {Key}. Waiting up to {Wait}s...", key, maxWait.TotalSeconds);

                var sw = System.Diagnostics.Stopwatch.StartNew();
                var waited = false;

                while (sw.Elapsed < maxWait)
                {
                    await Task.Delay(200); // küçük aralıklarla tekrar dene
                    if (await _concurrencyStore.TryEnterAsync(concurrentKey, maxConcurrent, window))
                    {
                        waited = true;
                        break;
                    }
                }

                if (!waited)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsync("Server is busy. Please try again later.");
                    _logger.LogWarning("Queued concurrency wait timeout for {Key}", key);
                    return;
                }
            }

            try
            {
                // Rate limit kontrolü
                var (allowed, current, remaining, resetSeconds) = await _rateStore.TryAcquireAsync(key, limit, window);

                context.Response.Headers["X-RateLimit-Limit"] = limit.ToString();
                context.Response.Headers["X-RateLimit-Remaining"] = remaining.ToString();
                context.Response.Headers["X-RateLimit-Reset"] = resetSeconds.ToString();

                if (!allowed)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.Response.Headers["Retry-After"] = resetSeconds.ToString();
                    await context.Response.WriteAsync("Too many requests. Try again later.");
                    _logger.LogWarning("Rate limit exceeded for key {Key}", key);
                    return;
                }

                await _next(context);
            }
            finally
            {
                // İş bitince concurrency slot’u serbest bırak
                await _concurrencyStore.ExitAsync(concurrentKey);
            }
        }

        private (string Key, int Limit, TimeSpan Window, int MaxConcurrent, TimeSpan MaxWait) ResolveKeyAndLimits(HttpContext ctx)
        {
            var userId = GetUserIdFromToken(ctx);
            if (!string.IsNullOrEmpty(userId))
            {
                return (
                    $"ratelimit:user:{userId}",
                    _options.UserPermitLimit,
                    TimeSpan.FromSeconds(_options.UserWindowSeconds),
                    _options.MaxConcurrentRequestsPerUser,
                    TimeSpan.FromSeconds(_options.MaxWaitSeconds)
                );
            }

            var ip = GetClientIp(ctx);
            return (
                $"ratelimit:ip:{ip}",
                _options.IpPermitLimit,
                TimeSpan.FromSeconds(_options.IpWindowSeconds),
                _options.MaxConcurrentRequestsPerIp,
                TimeSpan.FromSeconds(_options.MaxWaitSeconds)
            );
        }

        private string? GetUserIdFromToken(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                return null;

            var token = authHeader["Bearer ".Length..].Trim();

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                return jwt.Claims.FirstOrDefault(c =>
                    c.Type == ClaimTypes.NameIdentifier || c.Type == "sub" || c.Type == "userId")?.Value;
            }
            catch
            {
                return null;
            }
        }

        private string GetClientIp(HttpContext ctx)
        {
            if (ctx.Request.Headers.TryGetValue("X-Forwarded-For", out var header) && header.Count > 0)
            {
                var first = header[0].Split(',').First().Trim();
                if (!string.IsNullOrEmpty(first)) return first;
            }

            return ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }

}
