using ApplicationService.Services.Common;
using Microsoft.AspNetCore.Authorization;
using Shared.Constants;
using Shared.Exceptions;
using Shared.Static;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebAPI.Auth
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ICacheService _cache;
        public PermissionAuthorizationHandler(ICacheService cache)
        {
            _cache = cache;
        }
        protected async override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                throw new UnauthorizedException(ResponseCodes.UnauthorizedError);
            }

            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? context.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedException(ResponseCodes.UnauthorizedError);
            }

            string cacheKey = CacheKeys.Permission + userId;
            var permissions = await _cache.GetAsync<List<string>>(cacheKey);

            if (permissions == null || !permissions.Contains(requirement.Permission))
            {
                throw new UnauthorizedException(ResponseCodes.UnauthorizedError);
            }

            context.Succeed(requirement);
        }
    }
}
