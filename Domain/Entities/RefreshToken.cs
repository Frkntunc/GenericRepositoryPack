using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RefreshToken : Entity<RefreshToken, Guid>
    {
        public string Token { get; internal set; } = null!;
        public string UserId { get; internal set; } = null!;
        public DateTime ExpiryDate { get; internal set; }
        public bool IsRevoked { get; internal set; } = false;
        public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
    }

}
