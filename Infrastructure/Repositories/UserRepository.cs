using ApplicationService.Repositories;
using ApplicationService.Repositories.Common;
using System.Linq.Expressions;
using Domain.Entities;
using Infrastructure.Repositories.Common;
using Persistence.Contracts;

namespace Users.Infrastructure.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }
}
