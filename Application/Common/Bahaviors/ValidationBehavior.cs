

using FluentValidation;
using MediatR;

namespace Application.Common.Behavior;


public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> Validators) : IPipelineBehavior<TRequest, TResponse>
where TRequest : notnull
{
  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    var failures = Validators
    .Select(validator => validator.Validate(new ValidationContext<TRequest>(request)))
    .SelectMany(err => err.Errors)
    .Where(f => f is not null)
    .ToList();

    if (failures.Count != 0)
      throw new ValidationException(failures);

    return await next();
  }
}
