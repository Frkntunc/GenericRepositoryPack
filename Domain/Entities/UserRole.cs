using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Entities
{
    public class UserRole : Entity<UserRole, long>, IAuditableEntity
    {
        public long UserId { get; private set; }
        public User User { get; private set; }
        public long RoleId { get; private set; }
        public Role Role { get; private set; }
    }
}
