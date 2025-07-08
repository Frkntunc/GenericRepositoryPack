using ApplicationService.SharedKernel.Auth;

namespace WebAPI.Middleware
{
    public class JwtCookieMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtTokenService _tokenService;

        public JwtCookieMiddleware(RequestDelegate next, JwtTokenService tokenService)
        {
            _next = next;
            _tokenService = tokenService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Cookies["accessToken"];

            if (!string.IsNullOrEmpty(token))
            {
                var principal = _tokenService.ValidateToken(token);

                if (principal != null)
                {
                    context.User = principal;
                }
                else
                {
                    context.Response.Cookies.Delete("accessToken");
                    context.Response.Cookies.Delete("refreshToken");
                }
            }

            await _next(context);
        }
    }
}
