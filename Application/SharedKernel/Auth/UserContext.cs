using ApplicationService.SharedKernel.Auth.Common;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApplicationService.SharedKernel
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor accessor)
        {
            _httpContextAccessor = accessor;
        }

        public string UserId => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                                ?? _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                                ?? "";

        public string Email => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email)
                                ?? _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Email)
                                ?? "";

        public string Role => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role)
                                ?? "";
    }

}
