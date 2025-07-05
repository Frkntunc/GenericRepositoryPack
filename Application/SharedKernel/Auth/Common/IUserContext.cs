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
        string Email { get; }
        string Role { get; }
    }

}
