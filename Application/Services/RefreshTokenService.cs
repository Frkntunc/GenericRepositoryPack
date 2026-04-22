using ApplicationService.Repositories;
using ApplicationService.Services.Common;
using Domain.Entities;

namespace ApplicationService.Services
{
    public class RefreshTokenService : IScopedService
    {
        private readonly IRefreshTokenRepository refreshTokenRepository;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository)
        {
            this.refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<RefreshToken> CreateRefreshTokenAsync(string userId, CancellationToken cancellationToken = default)
        {
            var refreshToken = RefreshToken.Create(userId);

            await refreshTokenRepository.Add(refreshToken, cancellationToken);
            return refreshToken;
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await refreshTokenRepository.GetRefreshTokenAsync(token, cancellationToken);
        }

        public async Task<bool> ValidateRefreshTokenAsync(string token, string userId, CancellationToken cancellationToken = default)
        {
            return await refreshTokenRepository.ValidateRefreshTokenAsync(token, userId, cancellationToken);
        }

        public async Task RevokeRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            var refreshToken = await refreshTokenRepository.GetRefreshTokenAsync(token, cancellationToken);

            if (refreshToken != null)
            {
                refreshToken.ChangeRevokeStatus(true);

                refreshTokenRepository.Update(refreshToken);
            }
        }

        public async Task<RefreshToken?> RotateRefreshTokenAsync(string oldToken, string userId, CancellationToken cancellationToken = default)
        {
            var entity = await refreshTokenRepository.GetRefreshTokenAsync(oldToken, userId, cancellationToken);

            if (entity is null)
                return null;

            entity.ChangeRevokeStatus(true);

            var newToken = RefreshToken.Create(userId);

            await refreshTokenRepository.Add(newToken, cancellationToken);

            return newToken;
        }

        public async Task RevokeAllTokensByUserId(string userId, CancellationToken cancellationToken = default)
        {
            var tokens = await refreshTokenRepository.GetRefreshTokensByUserIdAsync(userId, cancellationToken);

            foreach (var token in tokens)
            {
                await RevokeRefreshTokenAsync(token.Token, cancellationToken);
            }

        }
    }
}
