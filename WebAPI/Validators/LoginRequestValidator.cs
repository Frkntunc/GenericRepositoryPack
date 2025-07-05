using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using WebAPI.Model.Login;

namespace WebAPI.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Mail boş olamaz.");
            RuleFor(x => x.Password).MinimumLength(4).WithMessage("Şifre en az 4 karakter olmalı.");
        }
    }
}
