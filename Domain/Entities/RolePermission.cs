using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RolePermission : Entity<RolePermission, long> , IAuditableEntity
    {
        public long RoleId { get; internal set; }
        public long PermissionId { get; internal set; }

        public Role Role { get; set; } = default!;
        public Permission Permission { get; set; } = default!;
    }
}
