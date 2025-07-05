using ApplicationService.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace ApplicationService.Repositories
{
    public interface IUserRepository : IWriteRepositoryBase<User>, IReadRepositoryBase<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(string id);
    }
}
