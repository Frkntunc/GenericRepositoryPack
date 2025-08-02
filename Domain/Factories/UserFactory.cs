using Domain.Entities;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Factories
{
    public static class UserFactory
    {
        public static User Create(string email, string firstName, string lastName, string passwordHash)
        {
            return new User
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PasswordHash = passwordHash,
                Status = StatusType.Available,
                LoginTryCount = 0,
                IsBlocked = false,
                LastPasswordChangeDate = null
            };
        }
    }

}
