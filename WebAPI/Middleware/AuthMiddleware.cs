using ApplicationService.SharedKernel.Auth;
using Microsoft.Extensions.Options;
using Shared.Constants;
using Shared.Contracts;
using Shared.Helpers;
using Shared.Options;

namespace Workify.WebAPI.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtTokenService _tokenService;
        private readonly string[] _anonymousPaths;

        public AuthMiddleware(RequestDelegate next, JwtTokenService tokenService, IOptions<AuthMiddlewareOptions> options)
        {
            _next = next;
            _tokenService = tokenService;
            _anonymousPaths = options.Value.AnonymousPaths;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";

            // Allow exact root path and predefined anonymous paths only
            if (path == "/" || _anonymousPaths.Any(p => path.StartsWith(p)))
            {
                await _next(context);
                return;
            }

            // JwtCookieMiddleware tarafindan zaten dogrulanmis istekleri gec
            if (context.User?.Identity?.IsAuthenticated == true)
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
                    await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, ResponseCodes.InvalidToken);
                    return;
                }
            }
            else
            {
                await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, ResponseCodes.MissingToken);
                return;
            }

            await _next(context);
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
}
