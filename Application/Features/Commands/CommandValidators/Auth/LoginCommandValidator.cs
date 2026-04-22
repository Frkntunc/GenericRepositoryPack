using ApplicationService.Features.Commands.CommandRequests.Auth;
using FluentValidation;
using Shared.Constants;

namespace ApplicationService.Features.Commands.CommandValidators.Auth
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithErrorCode(ResponseCodes.EmailCannotBeEmpty)
                .EmailAddress()
                .WithErrorCode(ResponseCodes.EnterValidEmail);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithErrorCode(ResponseCodes.PasswordRequired);

            RuleFor(x => x.IpAddress)
                .NotEmpty()
                .WithErrorCode(ResponseCodes.IpAddressRequired);
        }
    }
}
