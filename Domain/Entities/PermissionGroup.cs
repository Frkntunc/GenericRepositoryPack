using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PermissionGroup : Entity<PermissionGroup, int>, IAuditableEntity
    {
        public string Name { get; private set; }

        public ICollection<Permission> Permissions { get; private set; } = new List<Permission>();
    }
}
