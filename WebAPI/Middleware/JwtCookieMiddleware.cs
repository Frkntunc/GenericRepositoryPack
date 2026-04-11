using ApplicationService.SharedKernel.Auth;
using Microsoft.Extensions.Options;
using Shared.Options;

namespace WebAPI.Middleware
{
    public class JwtCookieMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtTokenService _tokenService;
        private readonly CookieTokenOptions _cookieTokenOptions;

        public JwtCookieMiddleware(RequestDelegate next, JwtTokenService tokenService, IOptions<CookieTokenOptions> cookieTokenOptions)
        {
            _next = next;
            _tokenService = tokenService;
            _cookieTokenOptions = cookieTokenOptions.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Cookies[_cookieTokenOptions.AccessTokenCookieName];

            if (!string.IsNullOrEmpty(token))
            {
                var principal = _tokenService.ValidateToken(token);

                if (principal != null)
                {
                    context.User = principal;
                    context.Items["CookieAuth"] = true;
                }
                else
                {
                    context.Response.Cookies.Delete(_cookieTokenOptions.AccessTokenCookieName);
                    context.Response.Cookies.Delete(_cookieTokenOptions.RefreshTokenCookieName);
                }
            }

            await _next(context);
        }
    }
}
