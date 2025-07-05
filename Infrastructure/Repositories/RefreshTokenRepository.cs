using ApplicationService.Repositories;
using Domain.Entities;
using Infrastructure.Contracts;
using Infrastructure.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class RefreshTokenRepository : WriteRepositoryBase<RefreshToken>, IRefreshTokenRepository
    {
        private readonly WriteDbContext _dbContext;
        public RefreshTokenRepository(WriteDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(RefreshToken refreshToken)
        {
            _dbContext.RefreshToken.Add(refreshToken);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string oldToken, string userId)
        {
            return await _dbContext.RefreshToken.FirstOrDefaultAsync(x =>
                x.Token == oldToken &&
                x.UserId == userId &&
                !x.IsRevoked &&
                x.ExpiryDate > DateTime.UtcNow);
        }

        public async Task<List<RefreshToken>> GetRefreshTokensByUserIdAsync(string userId)
        {
            return await _dbContext.RefreshToken
                .Where(rt => rt.UserId == userId && !rt.IsRevoked && !rt.IsExpired)
                .ToListAsync();
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var entity = await _dbContext.RefreshToken.FirstOrDefaultAsync(x => x.Token == token);
            if (entity != null)
            {
                entity.IsRevoked = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> ValidateRefreshTokenAsync(string token, string userId)
        {
            return await _dbContext.RefreshToken.AnyAsync(x =>
                x.Token == token &&
                x.UserId == userId &&
                !x.IsRevoked &&
                x.ExpiryDate > DateTime.UtcNow);
        }
    }
}
