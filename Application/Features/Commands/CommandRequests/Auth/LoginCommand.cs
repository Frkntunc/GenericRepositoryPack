using ApplicationService.Features.Common;
using MediatR;
using Shared.DTOs.Auth;
using Shared.DTOs.Common;

namespace ApplicationService.Features.Commands.CommandRequests.Auth
{
    public class LoginCommand : Command, IRequest<ServiceResponse<LoginResponse>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string IpAddress { get; set; }

        public LoginCommand(string email, string password, string ipAddress)
        {
            Email = email;
            IpAddress = ipAddress;
            Password = password;
        }
    }
}
