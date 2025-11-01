using ContractManager.Shared.Application.Exceptions;
using FluentValidation;
using MediatR;

namespace ContractManager.Shared.Application.Behaviors;
public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidatorBehavior(
        IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(_validators.Select(c => c.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(c => c.Errors).Where(c => c != null).ToList();

            if (failures.Count != 0)
            {
                throw new ActionValidationException(failures);
            }
        }

        return await next();
    }
}
