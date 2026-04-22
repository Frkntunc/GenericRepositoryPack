using ApplicationService.Features.Commands.CommandRequests.Auth;
using ApplicationService.Services;
using ApplicationService.Services.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Constants;
using Shared.DTOs.Common;
using Shared.Static;

namespace ApplicationService.Features.Commands.CommandHandlers.Auth
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<LogoutCommandHandler> _logger;
        private readonly RefreshTokenService _refreshTokenService;
        private readonly ICacheService _cache;

        public LogoutCommandHandler(
            ILogger<LogoutCommandHandler> logger,
            RefreshTokenService refreshTokenService,
            ICacheService cache)
        {
            _logger = logger;
            _refreshTokenService = refreshTokenService;
            _cache = cache;
        }

        public async Task<ServiceResponse<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await _refreshTokenService.RevokeAllTokensByUserId(request.UserId, cancellationToken);

            string cacheKey = CacheKeys.Permission + request.UserId;
            await _cache.RemoveAsync(cacheKey, cancellationToken);

            _logger.LogInformation("Kullanıcı çıkış yaptı. UserId: {UserId}", request.UserId);

            return ServiceResponse<bool>.Success(true, ResponseCodes.Success);
        }
    }
}
