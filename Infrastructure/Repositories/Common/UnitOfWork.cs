using ApplicationService.Repositories.Common;
using ApplicationService.Repositories;
using Infrastructure.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WriteDbContext _dbContext;
        public IUserRepository Users { get; }
        public IRefreshTokenRepository RefreshToken { get; }

        public UnitOfWork(WriteDbContext dbContext, IUserRepository users, IRefreshTokenRepository refreshToken)
        {
            _dbContext = dbContext;
            Users = users;
            RefreshToken = refreshToken;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
