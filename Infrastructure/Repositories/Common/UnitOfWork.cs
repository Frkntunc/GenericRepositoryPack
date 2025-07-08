using ApplicationService.Repositories.Common;
using ApplicationService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Persistence.Contracts;

namespace Infrastructure.Repositories.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        public IUserRepository Users { get; }
        public IRefreshTokenRepository RefreshToken { get; }

        public UnitOfWork(AppDbContext dbContext, IUserRepository users, IRefreshTokenRepository refreshToken)
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
