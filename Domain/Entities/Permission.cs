using Domain.Entities.Common;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Permission : Entity<Permission, long>, IAuditableEntity
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public int PermissionGroupId { get; private set; }

        public PermissionGroup PermissionGroup { get; private set; }
        public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();
    }
}
