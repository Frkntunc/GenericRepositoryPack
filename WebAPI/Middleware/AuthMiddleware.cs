using ApplicationService.SharedKernel.Auth;

namespace WebAPI.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtTokenService _tokenService;

        public AuthMiddleware(RequestDelegate next, JwtTokenService tokenService)
        {
            _next = next;
            _tokenService = tokenService;
        }

        private static readonly string[] _anonymousPaths = new[]
        {
                "/swagger",
                "/swagger/index.html",
                "/swagger/v1/swagger.json",
                "/api/login",
                "/api/register",
                "/"
        };

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";

            if (_anonymousPaths.Any(p => path.StartsWith(p)))
            {
                await _next(context);
                return;
            }

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(token))
            {
                var principal = _tokenService.ValidateToken(token);
                if (principal != null)
                {
                    context.User = principal;
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Geçersiz token.");
                    return;
                }
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token eksik.");
                return;
            }

            await _next(context);
        }
    }

}
