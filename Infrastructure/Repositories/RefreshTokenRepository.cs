using ApplicationService.Repositories;
using Domain.Entities;
using Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Persistence.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class RefreshTokenRepository : RepositoryBase<RefreshToken>, IRefreshTokenRepository
    {
        private readonly AppDbContext _dbContext;
        public RefreshTokenRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Add(RefreshToken refreshToken, CancellationToken cancellationToken = default)
        {
            _dbContext.RefreshToken.Add(refreshToken);
            return Task.CompletedTask;
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string oldToken, string userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.RefreshToken.FirstOrDefaultAsync(x =>
                x.Token == oldToken &&
                x.UserId == userId &&
                !x.IsRevoked &&
                x.ExpiryDate > DateTime.UtcNow, cancellationToken);
        }

        public async Task<List<RefreshToken>> GetRefreshTokensByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.RefreshToken
                .Where(rt => rt.UserId == userId && !rt.IsRevoked && !rt.IsExpired)
                .ToListAsync(cancellationToken);
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _dbContext.RefreshToken.FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
        }

        public async Task<bool> ValidateRefreshTokenAsync(string token, string userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.RefreshToken.AnyAsync(x =>
                x.Token == token &&
                x.UserId == userId &&
                !x.IsRevoked &&
                x.ExpiryDate > DateTime.UtcNow, cancellationToken);
        }
    }
}
