using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PermissionGroup : Entity<PermissionGroup, int>
    {
        public string Name { get; protected set; }

        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    }
}
