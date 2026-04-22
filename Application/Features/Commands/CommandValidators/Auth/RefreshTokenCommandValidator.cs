using ApplicationService.Features.Commands.CommandRequests.Auth;
using FluentValidation;
using Shared.Constants;

namespace ApplicationService.Features.Commands.CommandValidators.Auth
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithErrorCode(ResponseCodes.InvalidRefreshToken);
        }
    }
}
