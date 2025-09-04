using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Factories
{
    public static class RefreshTokenFactory
    {
        public static RefreshToken Create(string userId)
        {
            return new RefreshToken
            {
                Token = Guid.NewGuid().ToString("N"),
                UserId = userId,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };
        }
    }
}
