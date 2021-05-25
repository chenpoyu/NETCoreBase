using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using NETCoreBase.Core.Commands;
using NETCoreBase.Common;

namespace NETCoreBase.Core.Commands.Users
{
    public class LoginCommandValidator : AbstractValidator<LoginRequest>
    {
        public LoginCommandValidator()
        {
            RuleFor(command => command.UserName)
                .NotNull()
                .WithMessage("帳號不可為空")
                .NotEmpty()
                .WithMessage("帳號不可為空");

            RuleFor(command => command.UserPass)
                .NotNull()
                .WithMessage("密碼不可為空")
                .NotEmpty()
                .WithMessage("密碼不可為空");
        }
    }
}