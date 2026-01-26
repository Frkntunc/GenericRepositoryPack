using Domain.Entities;
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

                    var connection = _context.Database.GetDbConnection();
                    bool tableExists = false;

                    if (connection.State != System.Data.ConnectionState.Open)
                        connection.Open();

                    var tables = connection.GetSchema("Tables");

                    foreach (System.Data.DataRow row in tables.Rows)
                    {
                        if (row["TABLE_NAME"].ToString().Equals("DbVersionHistory", StringComparison.OrdinalIgnoreCase))
                        {
                            tableExists = true;
                            break;
                        }
                    }

                    if (connection.State == System.Data.ConnectionState.Open)
                        connection.Close();

                    if (tableExists)
                    {
                        _context.DbVersionHistory.Add(DbVersionHistory.Create(
                        
                            migration,
                            DateTime.Now,
                            "System",
                            "1.0.0",
                            Environment.MachineName,
                            stopwatch.Elapsed,
                            status,
                            errorMessage
                        ));

                        await _context.SaveChangesAsync();
                    }

                    if (status == "Failed")
                        throw new InvalidOperationException($"Migration {migration} failed: {errorMessage}");
                    else
                        _logger.LogInformation("Migration applied successfully: {Migration}", migration);
                }
            }
        }
    }

}
