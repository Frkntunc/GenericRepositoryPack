using Domain.Entities.Common;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Permission : Entity<Permission, long>
    {
        public string Name { get; internal set; }
        public string? Description { get; internal set; }
        public int PermissionGroupId { get; internal set; }

        public PermissionGroup PermissionGroup { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
