using Microsoft.Extensions.Options;
using Shared.Constants;
using Shared.Contracts;
using Shared.Helpers;
using Shared.Options;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;

namespace WebAPI.Middleware
{
    public class XssProtectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<XssProtectionMiddleware> _logger;
        private readonly XssProtectionOptions _options;
        private readonly IWebHostEnvironment _environment;
        private readonly SecurityHeaderOptions _securityHeaderOptions;

        private const int MaxRequestBodySize = 1_048_576; // 1 MB

        // HTML tag pattern: <script>, <img onerror=...>, <svg onload=...>, javascript:, on* event handlers vb.
        private static readonly Regex HtmlTagPattern = new(
            @"<\s*\/?\s*[a-zA-Z][a-zA-Z0-9]*(?:\s[^>]*)?\s*\/?\s*>|javascript\s*:|on\w+\s*=",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public XssProtectionMiddleware(
            RequestDelegate next,
            ILogger<XssProtectionMiddleware> logger,
            IOptions<XssProtectionOptions> options,
            IWebHostEnvironment environment,
            IOptions<SecurityHeaderOptions> securityHeaderOptions)
        {
            _next = next;
            _logger = logger;
            _options = options.Value;
            _environment = environment;
            _securityHeaderOptions = securityHeaderOptions.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Security header'ları ekle
            AddSecurityHeaders(context);

            if (_options.SanitizeRequests)
            {
                // Query string kontrolü
                if (context.Request.QueryString.HasValue)
                {
                    var decodedQuery = HttpUtility.UrlDecode(context.Request.QueryString.Value);
                    if (ContainsXssPayload(decodedQuery))
                    {
                        _logger.LogWarning("XSS payload query string'de tespit edildi. Path: {Path}", context.Request.Path);
                        await RejectRequest(context, ResponseCodes.XssDetectedInQueryString);
                        return;
                    }
                }

                // Body kontrolü
                if (HasReadableBody(context.Request))
                {
                    // Body size kontrolü
                    if (context.Request.ContentLength > MaxRequestBodySize)
                    {
                        _logger.LogWarning("Request body boyutu limiti aşıyor: {Size} bytes", context.Request.ContentLength);
                        await RejectRequest(context, ResponseCodes.RequestBodyTooLarge, StatusCodes.Status413PayloadTooLarge);
                        return;
                    }

                    context.Request.EnableBuffering();
                    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                    var body = await reader.ReadToEndAsync(context.RequestAborted);
                    context.Request.Body.Position = 0;

                    // Okunan body'nin gerçek boyutunu da kontrol et (chunked transfer için)
                    if (Encoding.UTF8.GetByteCount(body) > MaxRequestBodySize)
                    {
                        _logger.LogWarning("Request body boyutu limiti aşıyor (chunked).");
                        await RejectRequest(context, ResponseCodes.RequestBodyTooLarge, StatusCodes.Status413PayloadTooLarge);
                        return;
                    }

                    if (!string.IsNullOrWhiteSpace(body))
                    {
                        if (context.Request.ContentType?.Contains("application/json") == true)
                        {
                            if (!await ValidateJsonBody(body, context))
                                return;
                        }
                        else if (context.Request.ContentType?.Contains("application/x-www-form-urlencoded") == true)
                        {
                            var decodedBody = HttpUtility.UrlDecode(body);
                            if (ContainsXssPayload(decodedBody))
                            {
                                _logger.LogWarning("XSS payload form body'de tespit edildi. Path: {Path}", context.Request.Path);
                                await RejectRequest(context, ResponseCodes.XssDetectedInBody);
                                return;
                            }
                        }
                    }
                }
            }

            await _next(context);
        }

        private void AddSecurityHeaders(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                var headers = context.Response.Headers;

                headers["X-Content-Type-Options"] = "nosniff";
                headers["X-Frame-Options"] = "DENY";
                headers["X-XSS-Protection"] = "0";
                headers["Referrer-Policy"] = "no-referrer";
                headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";

                if (!_environment.IsDevelopment())
                {
                    headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
                }

                if (!headers.ContainsKey("Content-Security-Policy"))
                {
                    headers["Content-Security-Policy"] = _environment.IsDevelopment()
                        ? _securityHeaderOptions.DevelopmentCsp
                        : _securityHeaderOptions.ProductionCsp;
                }

                return Task.CompletedTask;
            });
        }

        private async Task<bool> ValidateJsonBody(string body, HttpContext context)
        {
            try
            {
                using var document = JsonDocument.Parse(body);
                if (ContainsXssInJsonElement(document.RootElement, out var fieldPath))
                {
                    _logger.LogWarning(
                        "XSS payload JSON body'de tespit edildi. Path: {Path}, Field: {Field}",
                        context.Request.Path, fieldPath);
                    await RejectRequest(context, ResponseCodes.XssDetectedInBody);
                    return false;
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Request body geçerli JSON değil. Path: {Path}", context.Request.Path);
                await RejectRequest(context, ResponseCodes.InvalidJsonFormat);
                return false;
            }

            return true;
        }

        private static bool ContainsXssInJsonElement(JsonElement element, out string fieldPath, string currentPath = "")
        {
            fieldPath = string.Empty;

            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    var value = element.GetString();
                    if (value != null && ContainsXssPayload(value))
                    {
                        fieldPath = currentPath;
                        return true;
                    }
                    break;

                case JsonValueKind.Object:
                    foreach (var property in element.EnumerateObject())
                    {
                        var path = string.IsNullOrEmpty(currentPath) ? property.Name : $"{currentPath}.{property.Name}";
                        if (ContainsXssInJsonElement(property.Value, out fieldPath, path))
                            return true;
                    }
                    break;

                case JsonValueKind.Array:
                    var index = 0;
                    foreach (var item in element.EnumerateArray())
                    {
                        var path = $"{currentPath}[{index}]";
                        if (ContainsXssInJsonElement(item, out fieldPath, path))
                            return true;
                        index++;
                    }
                    break;
            }

            return false;
        }

        private static bool ContainsXssPayload(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            var decoded = HttpUtility.HtmlDecode(input);
            return HtmlTagPattern.IsMatch(decoded);
        }

        private static bool HasReadableBody(HttpRequest request)
        {
            return (request.ContentLength == null || request.ContentLength > 0) &&
                   request.ContentType != null &&
                   (request.ContentType.Contains("application/json") ||
                    request.ContentType.Contains("application/x-www-form-urlencoded"));
        }

        private static async Task RejectRequest(HttpContext context, string responseCode, int httpStatus = StatusCodes.Status400BadRequest)
        {
            var apiResponse = new ApiResponse
            {
                Success = false,
                Status = httpStatus,
                Message = MessageResolver.GetResponseMessage(responseCode),
                ResponseCode = responseCode
            };
            context.Response.StatusCode = httpStatus;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(apiResponse);
        }
    }

    public class XssProtectionOptions
    {
        public bool SanitizeRequests { get; set; } = true;
    }
}