using ApplicationService.Features.Commands.CommandRequests.Auth;
using FluentValidation;
using Shared.Constants;

namespace ApplicationService.Features.Commands.CommandValidators.Auth
{
    public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithErrorCode(ResponseCodes.UserNotFound);
        }
    }
}
