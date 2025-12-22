using Domain.Entities.Common;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Role : Entity<Role,long>, IAuditableEntity
    {
        public string Name { get; internal set; }
        public string? Description { get; internal set; }
        public StatusType Status { get; internal set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
