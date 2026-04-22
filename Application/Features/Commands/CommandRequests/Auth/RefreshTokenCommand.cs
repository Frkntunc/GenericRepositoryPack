using ApplicationService.Features.Common;
using MediatR;
using Shared.DTOs.Auth;
using Shared.DTOs.Common;

namespace ApplicationService.Features.Commands.CommandRequests.Auth
{
    public class RefreshTokenCommand : Command, IRequest<ServiceResponse<RefreshTokenResponse>>
    {
        public string RefreshToken { get; set; }

        public RefreshTokenCommand(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }
}
