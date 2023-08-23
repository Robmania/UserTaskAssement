using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace DataBalk.Task.Api.Infrastructure.MediatR.PipelineBehaviours;

public class FluentValidationPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result, new()
{
    private readonly IEnumerable<IValidator<TRequest>> validators;

    public FluentValidationPipelineBehaviour(IEnumerable<IValidator<TRequest>> validators) => this.validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var results = new List<ValidationResult>();
        foreach (var validator in validators)
        {
            results.Add(await validator.ValidateAsync(request, cancellationToken));
        }

        var failures = results.SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            var result = new TResponse();
            result.SetFailures(failures.Select(f => f.ErrorMessage).ToList());
            return result;
        }

        return await next();
    }
}
