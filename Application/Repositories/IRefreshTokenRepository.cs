using ApplicationService.Repositories.Common;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Repositories
{
    public interface IRefreshTokenRepository : IRepositoryBase<RefreshToken>
    {
        Task Add(RefreshToken refreshToken, CancellationToken cancellationToken = default);
        Task<bool> ValidateRefreshTokenAsync(string token, string userId, CancellationToken cancellationToken = default);
        Task<RefreshToken> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
        Task<RefreshToken> GetRefreshTokenAsync(string oldToken, string userId, CancellationToken cancellationToken = default);
        Task<List<RefreshToken>> GetRefreshTokensByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    }
}
