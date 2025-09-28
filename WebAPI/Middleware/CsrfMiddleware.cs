using Microsoft.Extensions.Options;
using Shared.Options;
using System.IO;

namespace WebAPI.Middleware
{
    public class CsrfMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CsrfOptions _options;

        public CsrfMiddleware(RequestDelegate next, IOptions<CsrfOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant();

            if (!_options.MethodsToCheck.Contains(context.Request.Method.ToUpperInvariant()) ||
                _options.ExemptPaths.Any(p => path != null && path.StartsWith(p.ToLowerInvariant())) ||
                !context.User.Identity.IsAuthenticated)
            {
                await _next(context);
                return;
            }

            var csrfCookie = context.Request.Cookies[_options.CookieName];
            var csrfHeader = context.Request.Headers[_options.HeaderName].FirstOrDefault();

            if (string.IsNullOrEmpty(csrfCookie))
            {
                await RejectRequest(context, "CSRF cookie eksik.");
                return;
            }

            if (string.IsNullOrEmpty(csrfHeader))
            {
                await RejectRequest(context, "CSRF header eksik.");
                return;
            }

            if (csrfCookie != csrfHeader)
            {
                await RejectRequest(context, "CSRF token doğrulaması başarısız.");
                return;
            }

            await _next(context);
        }

        private static async Task RejectRequest(HttpContext context, string message)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync(message);
        }
    }
}
