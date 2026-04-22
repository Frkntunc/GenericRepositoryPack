using ApplicationService.Features.Commands.CommandRequests.Auth;
using ApplicationService.Repositories;
using ApplicationService.Services;
using ApplicationService.Services.Common;
using ApplicationService.SharedKernel.Auth;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Constants;
using Shared.DTOs.Auth;
using Shared.DTOs.Common;
using Shared.Options;
using Shared.Static;

namespace ApplicationService.Features.Commands.CommandHandlers.Auth
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ServiceResponse<RefreshTokenResponse>>
    {
        private readonly ILogger<RefreshTokenCommandHandler> _logger;
        private readonly RefreshTokenService _refreshTokenService;
        private readonly JwtTokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cache;
        private readonly JwtOptions _jwtOptions;

        public RefreshTokenCommandHandler(
            ILogger<RefreshTokenCommandHandler> logger,
            RefreshTokenService refreshTokenService,
            JwtTokenService tokenService,
            IUserRepository userRepository,
            ICacheService cache,
            IOptions<JwtOptions> jwtOptions)
        {
            _logger = logger;
            _refreshTokenService = refreshTokenService;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _cache = cache;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<ServiceResponse<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var existingToken = await _refreshTokenService.GetRefreshTokenAsync(request.RefreshToken, cancellationToken);

            if (existingToken is null || existingToken.IsRevoked || existingToken.IsExpired)
            {
                _logger.LogWarning("Geçersiz veya süresi dolmuş refresh token.");
                return ServiceResponse<RefreshTokenResponse>.Fail(ResponseCodes.InvalidRefreshToken);
            }

            var userId = existingToken.UserId;

            var rotatedToken = await _refreshTokenService.RotateRefreshTokenAsync(request.RefreshToken, userId, cancellationToken);

            if (rotatedToken is null)
            {
                _logger.LogWarning("Refresh token rotate edilemedi. UserId: {UserId}", userId);
                return ServiceResponse<RefreshTokenResponse>.Fail(ResponseCodes.InvalidRefreshToken);
            }

            string cacheKey = CacheKeys.Permission + userId;
            List<string>? userPermissions = await _cache.GetAsync<List<string>>(cacheKey, cancellationToken);

            if (userPermissions is null || userPermissions.Count == 0)
            {
                var user = await _userRepository.GetByIdWithRolesAsync(Convert.ToInt64(userId), cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning("Kullanıcı bulunamadı. UserId: {UserId}", userId);
                    return ServiceResponse<RefreshTokenResponse>.Fail(ResponseCodes.UserNotFound);
                }

                userPermissions = user.UserRoles
                    .SelectMany(ur => ur.Role?.RolePermissions ?? Enumerable.Empty<Domain.Entities.RolePermission>())
                    .Where(rp => rp.Permission != null && !string.IsNullOrEmpty(rp.Permission.Name))
                    .Select(rp => rp.Permission!.Name!)
                    .Distinct()
                    .ToList();

                await _cache.SetAsync(cacheKey, userPermissions, absoluteExpireTime: TimeSpan.FromMinutes(Convert.ToDouble(_jwtOptions.AccessTokenExpirationMinutes)), cancellationToken: cancellationToken);
            }

            var accessToken = _tokenService.GenerateToken(userId, userPermissions);
            var response = new RefreshTokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = rotatedToken.Token
            };

            return ServiceResponse<RefreshTokenResponse>.Success(response, ResponseCodes.Success);
        }
    }
}
