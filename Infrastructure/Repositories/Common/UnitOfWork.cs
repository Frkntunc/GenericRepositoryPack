using ApplicationService.Repositories;
using ApplicationService.Repositories.Common;
using ApplicationService.SharedKernel.Auth.Common;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Persistence.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserContext _currentUserService;

        public UnitOfWork(AppDbContext dbContext, IUserContext currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<int> SaveChangesAsync()
        {
            ApplyAuditing();

            return await _dbContext.SaveChangesAsync();
        }

        private void ApplyAuditing()
        {
            var entries = _dbContext.ChangeTracker.Entries()
                .Where(e => e.Entity is EntityBase &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

            var userId = string.IsNullOrEmpty(_currentUserService.UserId) ? 0L : Convert.ToInt64(_currentUserService.UserId);

            foreach (var entry in entries)
            {
                var entity = (EntityBase)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedOn = DateTime.Now;
                    entity.CreatedBy = userId;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.ModifiedOn = DateTime.Now;
                    entity.ModifiedBy = userId;
                }
            }
        }
    }
}
