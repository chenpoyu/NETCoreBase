using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using NETCoreBase.Common.Exceptions;

namespace NETCoreBase.Core.Behaviors
{
    public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidatorBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);
            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .GroupBy(c => c.PropertyName)
                .Select(g => new {
                    Key = g.Key, //g.Key.First().ToString().ToLower() + g.Key.Substring(1),
                    Errors = g.Select(c => c.ErrorMessage).Distinct().ToArray()
                })
                // .ToDictionary(t => t.Key, t => t)
                .ToDictionary(
                    kvp => kvp.Key.First().ToString().ToLower() + kvp.Key.Substring(1),
                    kvp => kvp.Errors.Select(e => e).Distinct().ToArray())
                ;
            if (failures.Any())
            {
                throw new CustomValidationException(failures);
            }

            return next(cancellationToken);

        }

    }
}
