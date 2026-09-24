using FluentValidation;
using MediatR;
using ApplicationValidationException = EduRate.Application.Common.Exceptions.ValidationException;

namespace EduRate.Application.Common.Behaviours
{
    /// <summary>
    /// MediatR pipeline behaviour: runs every registered FluentValidation validator for a
    /// request before its handler executes. Keeps validation out of Controllers and Handlers,
    /// and out of the Controller/Handler duplication the original code had.
    /// </summary>
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                var failures = validationResults
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .ToList();

                if (failures.Count != 0)
                {
                    throw new ApplicationValidationException(failures);
                }
            }

            return await next();
        }
    }
}
