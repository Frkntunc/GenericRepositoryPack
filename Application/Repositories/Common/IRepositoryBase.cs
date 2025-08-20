using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Repositories.Common
{
    public interface IRepositoryBase<T> where T : EntityBase
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> GetAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetByIdAsync(long id);

        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(IEnumerable<T> entities);

        Task RemoveAsync(T entity);
        Task RemoveRangeAsync(IEnumerable<T> entities);
    }
}
