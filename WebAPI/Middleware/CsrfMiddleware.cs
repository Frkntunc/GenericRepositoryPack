using ApplicationService.Services.Common;
using Microsoft.Extensions.Options;
using Shared.Constants;
using Shared.Contracts;
using Shared.Helpers;
using Shared.Options;

namespace WebAPI.Middleware
{
    public class CsrfMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CsrfOptions _options;
        private readonly ICsrfTokenService _csrfTokenService;

        public CsrfMiddleware(RequestDelegate next, IOptions<CsrfOptions> options, ICsrfTokenService csrfTokenService)
        {
            _next = next;
            _options = options.Value;
            _csrfTokenService = csrfTokenService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant();

            // Cookie-based auth kullanılmıyorsa (mobile client) CSRF kontrolü atla
            var isCookieAuth = context.Items.ContainsKey("CookieAuth");

            if (!isCookieAuth ||
                !_options.MethodsToCheck.Contains(context.Request.Method.ToUpperInvariant()) ||
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
                await RejectRequest(context, ResponseCodes.CsrfCookieMissing);
                return;
            }

            if (string.IsNullOrEmpty(csrfHeader))
            {
                await RejectRequest(context, ResponseCodes.CsrfHeaderMissing);
                return;
            }

            if (csrfCookie != csrfHeader)
            {
                await RejectRequest(context, ResponseCodes.CsrfValidationFailed);
                return;
            }

            if (!_csrfTokenService.ValidateToken(csrfCookie))
            {
                await RejectRequest(context, ResponseCodes.CsrfSignatureInvalid);
                return;
            }

            await _next(context);
        }

        private static async Task RejectRequest(HttpContext context, string responseCode)
        {
            var apiResponse = new ApiResponse
            {
                Success = false,
                Status = StatusCodes.Status403Forbidden,
                Message = MessageResolver.GetResponseMessage(responseCode),
                ResponseCode = responseCode
            };
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(apiResponse);
        }
    }
}
