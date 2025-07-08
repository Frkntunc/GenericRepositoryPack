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
    public class UserRole : Entity<UserRole,long>
    {
        public long UserId { get; protected set; }
        public User User { get; protected set; }
        public long RoleId { get; protected set; }
        public Role Role { get; protected set; }
    }
}
