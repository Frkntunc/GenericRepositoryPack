using Domain.Events.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
    public class UserCreatedEvent : Event
    {
        public string UserId { get; }
        public string Email { get; }
        public string FullName { get; }

        public UserCreatedEvent(string userId, string email, string fullName)
        {
            UserId = userId;
            Email = email;
            FullName = fullName;
        }
    }

}
