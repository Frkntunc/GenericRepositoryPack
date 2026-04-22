using ApplicationService.Services.Common;
using ApplicationService.SharedKernel.Auth.Common;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApplicationService.SharedKernel
{
    public class UserContext : IUserContext, IUserContextSetter, IScopedService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string? _overrideUserId;
        private string? _overrideRole;
        private string? _overrideIpAddress;

        public UserContext(IHttpContextAccessor accessor)
        {
            _httpContextAccessor = accessor;
        }

        public string UserId =>
            _overrideUserId
            ?? _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? _httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? "0";

        public string Role =>
            _overrideRole
            ?? _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);

        public string? IpAddress =>
            _overrideIpAddress
            ?? _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        public void SetUserId(string userId) => _overrideUserId = userId;
        public void SetRole(string role) => _overrideRole = role;
        public void SetIpAddress(string ipAddress) => _overrideIpAddress = ipAddress;
    }
}
