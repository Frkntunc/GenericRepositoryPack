using ApplicationService.Repositories.Common;
using Domain.Entities.Common;
using Infrastructure.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Common
{
    public class ReadRepositoryBase<T> : IReadRepositoryBase<T> where T : EntityBase
    {
        private readonly ReadDbContext _readDbContext;

        public ReadRepositoryBase(ReadDbContext readDbContext)
        {
            _readDbContext = readDbContext ?? throw new ArgumentNullException(nameof(readDbContext));
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _readDbContext.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _readDbContext.Set<T>().SingleOrDefaultAsync(predicate);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _readDbContext.Set<T>().FindAsync(id);
        }
    }
}
