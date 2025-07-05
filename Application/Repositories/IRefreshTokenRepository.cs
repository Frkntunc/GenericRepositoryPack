using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task Add(RefreshToken refreshToken);
        Task<bool> ValidateRefreshTokenAsync(string token, string userId);
        Task RevokeRefreshTokenAsync(string token);
        Task<RefreshToken> GetRefreshTokenAsync(string oldToken, string userId);
        Task<List<RefreshToken>> GetRefreshTokensByUserIdAsync(string userId);
    }
}
