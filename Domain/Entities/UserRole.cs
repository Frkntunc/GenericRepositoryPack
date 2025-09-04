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
        public long UserId { get; internal set; }
        public User User { get; internal set; }
        public long RoleId { get; internal set; }
        public Role Role { get; internal set; }
    }
}
