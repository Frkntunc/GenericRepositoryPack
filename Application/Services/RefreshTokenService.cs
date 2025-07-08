using ApplicationService.Repositories;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString("N"),
                UserId = userId,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };

            await refreshTokenRepository.Add(refreshToken);
            return refreshToken;
        }

        public async Task<bool> ValidateRefreshTokenAsync(string token, string userId)
        {
            return await refreshTokenRepository.ValidateRefreshTokenAsync(token, userId);
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            await refreshTokenRepository.RevokeRefreshTokenAsync(token);
        }

        public async Task<RefreshToken?> RotateRefreshTokenAsync(string oldToken, string userId)
        {
            var entity = await refreshTokenRepository.GetRefreshTokenAsync(oldToken, userId);

            if (entity is null)
                return null;

            entity.IsRevoked = true;

            var newToken = new RefreshToken
            {
                UserId = userId,
                Token = Guid.NewGuid().ToString("N"),
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };

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
