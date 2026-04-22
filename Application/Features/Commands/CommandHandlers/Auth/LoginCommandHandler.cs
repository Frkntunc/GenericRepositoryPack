using ApplicationService.Features.Commands.CommandRequests.Auth;
using ApplicationService.Repositories;
using ApplicationService.Services;
using ApplicationService.Services.Common;
using ApplicationService.SharedKernel.Auth;
using ApplicationService.SharedKernel.Auth.Common;
using Domain.Entities;
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
    public class LoginCommandHandler : IRequestHandler<LoginCommand, ServiceResponse<LoginResponse>>
    {
        private readonly ILogger<LoginCommandHandler> _logger;
        private readonly IHashService _hashService;
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenService _tokenService;
        private readonly RefreshTokenService _refreshTokenService;
        private readonly ICacheService _cache;
        private readonly JwtOptions _jwtOptions;

        public LoginCommandHandler(ILogger<LoginCommandHandler> logger, IHashService hashService, IUserRepository userRepository, JwtTokenService tokenService, RefreshTokenService refreshTokenService, ICacheService cache, IOptions<JwtOptions> jwtOptions)
        {
            _logger = logger;
            _hashService = hashService;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
            _cache = cache;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<ServiceResponse<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Email veya şifre hatalı");
                return ServiceResponse<LoginResponse>.Fail(ResponseCodes.EmailOrPasswordWrong);
            }

            var hashPassword = _hashService.HashWithSalt(request.Password);

            if (hashPassword != user.PasswordHash)
            {
                _logger.LogWarning("Email veya şifre hatalı");
                return ServiceResponse<LoginResponse>.Fail(ResponseCodes.EmailOrPasswordWrong);
            }

            user.RecordLogin(request.IpAddress);
            _userRepository.Update(user);

            List<string> userPermissions = user.UserRoles
                .SelectMany(ur => ur.Role?.RolePermissions ?? Enumerable.Empty<RolePermission>())
                .Where(rp => rp.Permission != null && !string.IsNullOrEmpty(rp.Permission.Name))
                .Select(rp => rp.Permission!.Name!)
                .Distinct()
                .ToList();

            string cacheKey = CacheKeys.Permission + user.Id.ToString();
            await _cache.SetAsync(cacheKey, userPermissions, absoluteExpireTime: TimeSpan.FromMinutes(Convert.ToDouble(_jwtOptions.AccessTokenExpirationMinutes)), cancellationToken: cancellationToken);

            var accessToken = _tokenService.GenerateToken(user.Id.ToString(), userPermissions);
            var refreshToken = await _refreshTokenService.CreateRefreshTokenAsync(user.Id.ToString(), cancellationToken);
            var response = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };

            return ServiceResponse<LoginResponse>.Success(response, ResponseCodes.Success);
        }
    }
}
