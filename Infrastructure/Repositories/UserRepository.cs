using ApplicationService.Repositories;
using ApplicationService.Repositories.Common;
using System.Linq.Expressions;
using Domain.Entities;

namespace Users.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private static readonly List<User> _users = new();

    public Task AddAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public Task AddRangeAsync(IEnumerable<User> entities)
    {
        throw new NotImplementedException();
    }

    public Task<List<User>> GetAllAsync() => Task.FromResult(_users);

    public Task<User> GetAsync(Expression<Func<User, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public Task RemoveRangeAsync(IEnumerable<User> entities)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateRangeAsync(IEnumerable<User> entities)
    {
        throw new NotImplementedException();
    }

    Task<IReadOnlyList<User>> IReadRepositoryBase<User>.GetAllAsync()
    {
        throw new NotImplementedException();
    }
}
