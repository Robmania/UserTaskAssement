using DataBalk.Task.Api.Infrastructure.MediatR;
using MediatR;
using FluentValidation;
using DataBalk.Task.Api.Infrastructure.Validators;
using DataBalk.Task.Api.Enums;
using System.Text.Json.Serialization;
using DataBalk.Task.Api.Constants;
using AutoMapper;
using DataBalk.Task.Api.Features.Tasks.Models;
using DataBalk.Task.Api.Data.Repositories;

namespace DataBalk.Task.Api.Features.Tasks.Commands;

public class AddOrUpdateTaskCommand : AppCommand, IRequest<Result<Data.Entities.Task>>
{
    public long Assignee { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }

    public override string CommandDescription => "Add or update a Task";

    [JsonIgnore]
    public Data.Entities.Task? Task { get; set; }
}

public class AddOrUpdateTaskCommandValidator : AbstractValidator<AddOrUpdateTaskCommand>
{
    private readonly ITaskRepository _taskRepository;

    public AddOrUpdateTaskCommandValidator(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
        RuleFor(cmd => cmd).OperationMustMatchId(cmd => cmd.Id);
        RuleFor(cmd => cmd).MustAsync(TaskMustExistInDb)
            .WithMessage(cmd => string.Format(TaskConstants.ValidationMessages.DoesNotExist, "Task", cmd.Id))
            .DependentRules(
                () =>
                {
                    RuleFor(qry => qry.Title)
                        .NotEmpty()
                        .WithMessage(TaskConstants.ValidationMessages.RequiredField(nameof(AddOrUpdateTaskCommand.Title)));

                    RuleFor(qry => qry.Description)
                        .NotEmpty()
                        .WithMessage(TaskConstants.ValidationMessages.RequiredField(nameof(AddOrUpdateTaskCommand.Description)));

                    RuleFor(qry => qry.Assignee)
                        .NotEmpty()
                        .WithMessage(TaskConstants.ValidationMessages.RequiredField(nameof(AddOrUpdateTaskCommand.Assignee)));
                });
    }

    private async Task<bool> TaskMustExistInDb(AddOrUpdateTaskCommand cmd, CancellationToken ctx)
    {
        cmd.Task = cmd.Operation == EOperation.Add ? new Data.Entities.Task() : await _taskRepository.GetTaskByIdAsync(cmd.Id, ctx);

        return cmd.Task != null;
    }
}

public class AddOrUpdateTaskCommandHandler : IRequestHandler<AddOrUpdateTaskCommand, Result<Data.Entities.Task>>
{
    private readonly ITaskRepository _taskRepository;

    public AddOrUpdateTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Result<Data.Entities.Task>> Handle(AddOrUpdateTaskCommand cmd, CancellationToken ctx)
    {
        cmd.Task.Title = cmd.Title;
        cmd.Task.Description = cmd.Description;
        cmd.Task.Assignee = cmd.Assignee;
        cmd.Task.DueDate = cmd.DueDate;

        if (cmd.Operation == EOperation.Add)
        {
            await _taskRepository.CreateTaskAsync(cmd.Task, ctx);
        }
        else
        {
            await _taskRepository.UpdateTaskAsync(cmd.Task, ctx);
        }

        return new SuccessResult<Data.Entities.Task>(cmd.Task);
    }
}

public class AddOrUpdateTaskCommandMappingProfile : Profile
{
    public AddOrUpdateTaskCommandMappingProfile()
    {
        CreateMap<TaskAddOrUpdateModel, AddOrUpdateTaskCommand>();
    }
}