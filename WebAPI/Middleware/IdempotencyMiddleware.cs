using Microsoft.Extensions.Caching.Distributed;
using Shared.Constants;
using Shared.Contracts;
using Shared.Helpers;
using StackExchange.Redis;
using System.Text.Json;

namespace WebAPI.Middleware
{
    public class IdempotencyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<IdempotencyMiddleware> _logger;

        // Lua script: atomik olarak "eğer key yoksa processing olarak set et" işlemi yapar.
        // SETNX benzeri ama durum alanı (status) ile birlikte.
        // Return: 1 = kilit alındı (ilk istek), 0 = zaten var (duplicate)
        private const string AcquireLockScript = @"
            local existing = redis.call('GET', KEYS[1])
            if existing then
                return existing
            end
            local value = cjson.encode({status='processing', response=cjson.null, statusCode=0})
            redis.call('SET', KEYS[1], value, 'EX', tonumber(ARGV[1]))
            return nil";

        // Lua script: atomik olarak durumu güncelle (completed/failed + response).
        private const string UpdateStatusScript = @"
            local existing = redis.call('GET', KEYS[1])
            if not existing then
                return 0
            end
            local value = cjson.encode({status=ARGV[1], response=ARGV[2], statusCode=tonumber(ARGV[3])})
            redis.call('SET', KEYS[1], value, 'EX', tonumber(ARGV[4]))
            return 1";

        public IdempotencyMiddleware(
            RequestDelegate next,
            IDistributedCache cache,
            IConnectionMultiplexer redis,
            ILogger<IdempotencyMiddleware> logger)
        {
            _next = next;
            _cache = cache;
            _redis = redis;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var attr = endpoint?.Metadata.GetMetadata<IdempotentAttribute>();

            if (attr == null)
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("Idempotency-Key", out var key) ||
                string.IsNullOrWhiteSpace(key))
            {
                await WriteErrorResponse(context, StatusCodes.Status400BadRequest, ResponseCodes.IdempotencyKeyMissing);
                return;
            }

            var cacheKey = $"idempotency:{key}";
            var db = _redis.GetDatabase();
            var ttlSeconds = attr.ExpirationSeconds;

            // Atomik kilit alma: Lua script ile race condition'ı engelle
            var result = await db.ScriptEvaluateAsync(
                AcquireLockScript,
                new RedisKey[] { cacheKey },
                new RedisValue[] { ttlSeconds });

            if (!result.IsNull)
            {
                // Key zaten var — mevcut durumu kontrol et
                var existing = JsonSerializer.Deserialize<IdempotencyRecord>(result.ToString()!);

                if (existing?.Status == "processing")
                {
                    // Başka bir istek hâlâ işleniyor
                    _logger.LogWarning("Idempotency key {Key} is currently being processed", key);
                    await WriteErrorResponse(context, StatusCodes.Status409Conflict, ResponseCodes.RequestAlreadyProcessing);
                    return;
                }

                if (existing?.Status == "completed")
                {
                    // Daha önce başarıyla tamamlanmış — cache'lenmiş cevabı dön
                    _logger.LogInformation("Returning cached response for idempotency key {Key}", key);
                    context.Response.StatusCode = existing.StatusCode;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(existing.Response ?? "");
                    return;
                }

                // Status == "failed" → kilidi serbest bırak, yeniden denenebilir
                // Key'i sil ki retry yapılabilsin
                _logger.LogInformation("Previous request with key {Key} failed, allowing retry", key);
                await db.KeyDeleteAsync(cacheKey);

                // Tekrar kilit almayı dene
                var retryResult = await db.ScriptEvaluateAsync(
                    AcquireLockScript,
                    new RedisKey[] { cacheKey },
                    new RedisValue[] { ttlSeconds });

                if (!retryResult.IsNull)
                {
                    // Başka bir istek araya girdi
                    await WriteErrorResponse(context, StatusCodes.Status409Conflict, ResponseCodes.RequestAlreadyProcessing);
                    return;
                }
            }

            // Kilit alındı — pipeline'ı çalıştır
            var originalBody = context.Response.Body;
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            try
            {
                await _next(context);

                memoryStream.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(memoryStream).ReadToEndAsync(context.RequestAborted);
                var statusCode = context.Response.StatusCode;

                // Başarı/başarısızlık durumuna göre cache'i güncelle
                var isSuccess = statusCode >= 200 && statusCode < 300;
                var status = isSuccess ? "completed" : "failed";

                await db.ScriptEvaluateAsync(
                    UpdateStatusScript,
                    new RedisKey[] { cacheKey },
                    new RedisValue[] { status, responseText, statusCode, ttlSeconds });

                if (!isSuccess)
                {
                    _logger.LogWarning(
                        "Request with idempotency key {Key} failed with status {StatusCode}, marked as failed for retry",
                        key, statusCode);
                }

                // Response'u client'a gönder
                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(originalBody, context.RequestAborted);
                context.Response.Body = originalBody;
            }
            catch (Exception ex)
            {
                // Exception durumunda: failed olarak işaretle
                _logger.LogError(ex, "Exception during idempotent request {Key}, marking as failed", key);

                await db.ScriptEvaluateAsync(
                    UpdateStatusScript,
                    new RedisKey[] { cacheKey },
                    new RedisValue[] { "failed", "", 500, ttlSeconds });

                context.Response.Body = originalBody;
                throw; // Exception'ı yukarı fırlat (global error handler yakalasın)
            }
        }

        private static async Task WriteErrorResponse(HttpContext context, int statusCode, string responseCode)
        {
            var apiResponse = new ApiResponse
            {
                Success = false,
                Status = statusCode,
                Message = MessageResolver.GetResponseMessage(responseCode),
                ResponseCode = responseCode
            };
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(apiResponse);
        }
    }

    internal sealed class IdempotencyRecord
    {
        public string Status { get; set; } = "";
        public string? Response { get; set; }
        public int StatusCode { get; set; }
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
