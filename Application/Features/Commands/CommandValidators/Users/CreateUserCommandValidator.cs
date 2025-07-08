using ApplicationService.Features.Commands.CommandRequests.Users;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Features.Commands.CommandValidators.Users
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(u => u.FirstName).NotEmpty().WithMessage("İsim alanı zorunludur.");
            RuleFor(u => u.LastName).NotEmpty().WithMessage("Soyisim alanı zorunludur.");
            RuleFor(u => u.Email).NotEmpty().EmailAddress();
        }
    }
}
