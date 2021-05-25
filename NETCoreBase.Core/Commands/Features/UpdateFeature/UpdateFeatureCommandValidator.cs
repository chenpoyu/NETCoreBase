using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using NETCoreBase.Core.Commands;
using NETCoreBase.Common;

namespace NETCoreBase.Core.Commands.Features
{
    public class UpdateFeatureCommandValidator : AbstractValidator<UpdateFeatureRequest>
    {
        public UpdateFeatureCommandValidator()
        {
            RuleFor(command => command.Id)
                .NotNull()
                .WithMessage("功能編號驗證錯誤");

            RuleFor(command => command.Name)
                .Length(0, 30)
                .WithMessage("功能名稱的長度必須小於 30。");

            RuleFor(command => command.Path)
                .Length(0, 100)
                .WithMessage("功能路徑的長度必須小於 100。");

            RuleFor(command => command.Status)
                .Must(c => c == "A" || c == "D" || c == "U")
                .WithMessage("狀態格式錯誤。");

        }
    }
}