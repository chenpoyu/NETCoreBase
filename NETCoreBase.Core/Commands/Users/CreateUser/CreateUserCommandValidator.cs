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
    public class CreateUserCommandValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserCommandValidator()
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

            RuleFor(command => command.NormalizedUserName)
                .Length(0, 256)
                .WithMessage("使用者姓名的長度必須小於 256。");

            RuleFor(command => command.Email)
                .EmailAddress()
                .WithMessage("使用者電子郵件格式錯誤。")
                .Length(0, 256)
                .WithMessage("使用者電子郵件的長度必須小於 256。");

            RuleFor(command => command.PhoneNumber)
                .Matches(Const.PHONE_REGEX)
                .WithMessage("使用者手機格式錯誤。")
                .Length(0, 10)
                .WithMessage("使用者手機的長度必須為10碼。");
        }
    }
}