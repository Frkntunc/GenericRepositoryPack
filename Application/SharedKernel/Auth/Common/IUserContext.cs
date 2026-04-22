using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.SharedKernel.Auth.Common
{
    public interface IUserContext
    {
        string UserId { get; }
        string Role { get; }
        string? IpAddress { get; }
    }

    public interface IUserContextSetter
    {
        void SetUserId(string userId);
        void SetRole(string role);
        void SetIpAddress(string ipAddress);
    }
}
