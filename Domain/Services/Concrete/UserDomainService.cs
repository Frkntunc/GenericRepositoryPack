using Domain.Entities;
using Domain.Services.Abstract;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Concrete
{
    public class UserDomainService : IUserDomainService
    {
        public void ChangeStatus(User user, StatusType newStatus)
        {
            user.Status = newStatus;
        }

        public void BlockUser(User user)
        {
            user.IsBlocked = true;
        }
    }
}
