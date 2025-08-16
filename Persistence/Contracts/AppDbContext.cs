using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Contracts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<RefreshToken> RefreshToken => Set<RefreshToken>();
        public DbSet<User> User => Set<User>();
        public DbSet<Role> Role => Set<Role>();
        public DbSet<Permission> Permission => Set<Permission>();
        public DbSet<UserRole> UserRole => Set<UserRole>();
        public DbSet<RolePermission> RolePermission => Set<RolePermission>();
        public DbSet<PermissionGroup> PermissionGroup => Set<PermissionGroup>();
        public DbSet<DbVersionHistory> DbVersionHistory => Set<DbVersionHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
