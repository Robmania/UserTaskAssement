using DataBalk.Task.Api.Constants;
using DataBalk.Task.Api.Enums;
using DataBalk.Task.Api.Infrastructure.MediatR;
using FluentValidation;

namespace DataBalk.Task.Api.Infrastructure.Validators;

public static class CommandCustomValidators
{
    public static IRuleBuilderOptions<T, T> OperationMustMatchId<T>(this IRuleBuilder<T, T> ruleBuilder, Func<T, long>? idValueGetter = null) where T : AppCommand
    {
        var defaultIdValueGetter = (T c) => c.Id;
        idValueGetter = idValueGetter ?? defaultIdValueGetter;
        return ruleBuilder
            .Must(cmd => cmd.Operation == EOperation.Add && idValueGetter(cmd) == 0 ||
                         cmd.Operation == EOperation.Update && idValueGetter(cmd) > 0)
            .WithMessage(cmd =>
                cmd.Operation == EOperation.NotSpecified
                    ? "You have not specified an 'Add' or 'Update' operation for this Command."
                    : string.Format(TaskConstants.ValidationMessages.OperationDoesNotMatchId, cmd.Operation));
    }
}
