using DataBalk.Task.Api.Constants;
using DataBalk.Task.Api.Data.Repositories;
using DataBalk.Task.Api.Infrastructure.MediatR;
using FluentValidation;
using MediatR;

namespace DataBalk.Task.Api.Features.Tasks.Commands;

public class DeleteTaskCommand: AppCommand, IRequest<Result>
{
    public override string CommandDescription => "Delete an existing Task";
}

public class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
{
    public DeleteTaskCommandValidator()
    {
        RuleFor(qry => qry.Id)
            .NotEmpty()
            .WithMessage(TaskConstants.ValidationMessages.RequiredField(nameof(DeleteTaskCommand.Id)));
    }
}

public class DeleteTaskCommandHandler: IRequestHandler<DeleteTaskCommand, Result>
{
    private readonly ITaskRepository _taskRepository;

    public DeleteTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Result> Handle(DeleteTaskCommand cmd, CancellationToken ctx)
    {
        await _taskRepository.DeleteTaskAsync(cmd.Id, ctx);

        return new SuccessResult();
    }
}
