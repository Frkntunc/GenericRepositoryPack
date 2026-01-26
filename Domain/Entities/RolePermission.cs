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
        public long RoleId { get; private set; }
        public long PermissionId { get; private set; }

        public Role Role { get; private set; } = default!;
        public Permission Permission { get; private set; } = default!;
    }
}
