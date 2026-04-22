using ApplicationService.Features.Common;
using MediatR;
using Shared.DTOs.Common;

namespace ApplicationService.Features.Commands.CommandRequests.Auth
{
    public class LogoutCommand : Command, IRequest<ServiceResponse<bool>>
    {
        public string UserId { get; set; }

        public LogoutCommand(string userId)
        {
            UserId = userId;
        }
    }
}
