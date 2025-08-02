using ApplicationService.Repositories;
using Domain.Entities;
using Domain.Entities.Common;
using Domain.Factories;
using Domain.Services.Abstract;
using Domain.Services.Concrete;
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
        private readonly IRefreshTokenDomainService refreshTokenDomainService;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, IRefreshTokenDomainService refreshTokenDomainService)
        {
            this.refreshTokenRepository = refreshTokenRepository;
            this.refreshTokenDomainService = refreshTokenDomainService;
        }

        public async Task<RefreshToken> CreateRefreshTokenAsync(string userId)
        {
            var refreshToken = RefreshTokenFactory.Create(userId);

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
                refreshTokenDomainService.ChangeRevokeStatus(refreshToken, true);

                await refreshTokenRepository.UpdateAsync(refreshToken);
            }
        }

        public async Task<RefreshToken?> RotateRefreshTokenAsync(string oldToken, string userId)
        {
            var entity = await refreshTokenRepository.GetRefreshTokenAsync(oldToken, userId);

            if (entity is null)
                return null;

            refreshTokenDomainService.ChangeRevokeStatus(entity, true);

            var newToken = RefreshTokenFactory.Create(userId);

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
