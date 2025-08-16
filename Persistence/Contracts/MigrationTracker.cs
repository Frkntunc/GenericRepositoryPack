using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Contracts
{
    public class MigrationTracker
    {
        private readonly AppDbContext _context;
        private readonly IMigrator _migrator;

        public MigrationTracker(AppDbContext context, IMigrator migrator)
        {
            _context = context;
            _migrator = migrator;
        }

        public async Task ApplyMigrationsAsync()
        {
            var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();

            foreach (var migration in pendingMigrations)
            {
                await _migrator.MigrateAsync(migration);

                _context.DbVersionHistory.Add(new DbVersionHistory
                {
                    MigrationName = migration,
                    AppliedOn = DateTime.Now
                });
            }

            if (_context.ChangeTracker.HasChanges())
            {
                await _context.SaveChangesAsync();
            }
        }
    }

}
