using Ganss.Xss;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebAPI.Middleware
{
    public class XssProtectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<XssProtectionMiddleware> _logger;
        private readonly XssProtectionOptions _options;
        private static readonly HtmlSanitizer _sanitizer = new HtmlSanitizer();
        private readonly IWebHostEnvironment _environment;

        public XssProtectionMiddleware(RequestDelegate next, ILogger<XssProtectionMiddleware> logger, IOptions<XssProtectionOptions> options, IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _options = options.Value;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                context.Response.Headers["X-Frame-Options"] = "DENY";
                context.Response.Headers["Referrer-Policy"] = "no-referrer";
                context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=()";

                if (!context.Response.Headers.ContainsKey("Content-Security-Policy"))
                {
                    if (_environment.IsDevelopment())
                    {
                        // Swagger için daha gevşek CSP
                        context.Response.Headers["Content-Security-Policy"] =
                            "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; connect-src 'self' ws://localhost:* wss://localhost:*;";
                    }
                    else
                    {
                        // Prod için sıkı CSP
                        // CSP (Content-Security-Policy) ile sadece güvenli kaynaklardan script çalışmasına izin verilir
                        context.Response.Headers["Content-Security-Policy"] =
                            "default-src 'self'; script-src 'self'; object-src 'none'; frame-ancestors 'none';";
                    }
                }
                return Task.CompletedTask;
            });

            // Request içeriğini sanitize et
            context.Request.EnableBuffering();
            if (context.Request.ContentLength > 0 &&
                (context.Request.ContentType?.Contains("application/json") == true ||
                 context.Request.ContentType?.Contains("application/x-www-form-urlencoded") == true))
            {
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (!string.IsNullOrWhiteSpace(body))
                {
                    try
                    {
                        var sanitized = SanitizeInput(body);
                        var bytes = Encoding.UTF8.GetBytes(sanitized);
                        context.Request.Body = new MemoryStream(bytes);
                        _logger.LogDebug("XSS Middleware: Request body sanitize edildi.");
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning(ex, "Request body sanitize edilirken JSON hatası oluştu.");
                    }
                }
            }

            await _next(context);
        }

        private static string SanitizeInput(string input)
        {
            return _sanitizer.Sanitize(input);
        }
    }

    public class XssProtectionOptions
    {
        public bool SanitizeRequests { get; set; } = true;
    }

    public static class XssProtectionMiddlewareExtensions
    {
        public static IApplicationBuilder UseXssProtection(this IApplicationBuilder builder, Action<XssProtectionOptions>? configureOptions = null)
        {
            var options = new XssProtectionOptions();
            configureOptions?.Invoke(options);
            return builder.UseMiddleware<XssProtectionMiddleware>(Options.Create(options));
        }
    }
}