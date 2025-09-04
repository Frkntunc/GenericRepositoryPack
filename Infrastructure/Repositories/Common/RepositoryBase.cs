using ApplicationService.Repositories.Common;
using Domain.Entities.Common;
using Persistence.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Common
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : EntityBase
    {
        private readonly AppDbContext _dbContext;

        public RepositoryBase(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddAsync(T entity)
        {
            var result = _dbContext.Set<T>().Add(entity);
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().AsNoTracking().ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().SingleOrDefaultAsync(predicate);
        }

        public async Task<T> GetByIdAsync(long id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task RemoveAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public async Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;

        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            _dbContext.Entry(entities).State = EntityState.Modified;
        }
    }
}
