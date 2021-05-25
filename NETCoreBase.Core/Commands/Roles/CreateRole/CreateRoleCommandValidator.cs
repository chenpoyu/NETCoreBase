using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using NETCoreBase.Core.Commands;
using NETCoreBase.Common;

namespace NETCoreBase.Core.Commands.Roles
{
    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleRequest>
    {
        public CreateRoleCommandValidator()
        {
            RuleFor(command => command.Name)
                .Length(0, 256)
                .WithMessage("角色名稱（英文）的長度必須小於 256。");

            RuleFor(command => command.NormalizedName)
                .Length(0, 256)
                .WithMessage("角色名稱（中文）的長度必須小於 256。");
        }
    }
}