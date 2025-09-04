﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Contracts
{
    public class MigrationTracker
    {
        private readonly AppDbContext _context;
        private readonly IMigrator _migrator;
        private readonly ILogger<MigrationTracker> _logger;

        public MigrationTracker(AppDbContext context, IMigrator migrator, ILogger<MigrationTracker> logger)
        {
            _context = context;
            _migrator = migrator;
            _logger = logger;
        }

        public async Task ApplyMigrationsAsync()
        {
            var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();

            foreach (var migration in pendingMigrations)
            {
                _logger.LogInformation("Applying migration: {Migration}", migration);

                var stopwatch = Stopwatch.StartNew();
                var status = "Success";
                string errorMessage = null;

                try
                {
                    await _migrator.MigrateAsync(migration);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Migration failed!");
                    status = "Failed";
                    errorMessage = ex.Message;
                }
                finally
                {
                    stopwatch.Stop();

                    _context.DbVersionHistory.Add(new DbVersionHistory
                    {
                        MigrationName = migration,
                        AppliedOn = DateTime.Now,
                        AppliedBy = "System",
                        AppVersion = "1.0.0",
                        MachineName = Environment.MachineName,
                        Duration = stopwatch.Elapsed,
                        Status = status,
                        ErrorMessage = errorMessage
                    });

                    await _context.SaveChangesAsync();

                    if (status == "Failed")
                        throw new InvalidOperationException($"Migration {migration} failed: {errorMessage}");
                    else
                        _logger.LogInformation("Migration applied successfully: {Migration}", migration);
                }
            }
        }
    }

}
