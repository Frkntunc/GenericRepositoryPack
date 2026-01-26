using ApplicationService.Repositories;
using Domain.Entities;

namespace ApplicationService.Services
{
    public class RefreshTokenService
    {
        private readonly IRefreshTokenRepository refreshTokenRepository;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository)
        {
            this.refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<RefreshToken> CreateRefreshTokenAsync(string userId)
        {
            var refreshToken = RefreshToken.Create(userId);

            await refreshTokenRepository.Add(refreshToken);
            return refreshToken;
        }

        public async Task<bool> ValidateRefreshTokenAsync(string token, string userId)
        {
            return await refreshTokenRepository.ValidateRefreshTokenAsync(token, userId);
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await refreshTokenRepository.GetRefreshTokenAsync(token);

            if (refreshToken != null)
            {
                refreshToken.ChangeRevokeStatus(true);

                await refreshTokenRepository.UpdateAsync(refreshToken);
            }
        }

        public async Task<RefreshToken?> RotateRefreshTokenAsync(string oldToken, string userId)
        {
            var entity = await refreshTokenRepository.GetRefreshTokenAsync(oldToken, userId);

            if (entity is null)
                return null;

            entity.ChangeRevokeStatus(true);

            var newToken = RefreshToken.Create(userId);

            await refreshTokenRepository.Add(newToken);

            return newToken;
        }

        public async Task RevokeAllTokensByUserId(string userId)
        {
            var tokens = await refreshTokenRepository.GetRefreshTokensByUserIdAsync(userId);

            foreach (var token in tokens)
            {
                RevokeRefreshTokenAsync(token.Token);
            }

        }
    }
}
