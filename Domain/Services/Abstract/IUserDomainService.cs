using Domain.Entities;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Abstract
{
    public interface IUserDomainService
    {
        void ChangeStatus(User user, StatusType newStatus);
        void BlockUser(User user);
    }

}
