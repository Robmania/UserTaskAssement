using DataBalk.Task.Api.Constants;
using DataBalk.Task.Api.Data;
using DataBalk.Task.Api.Data.Repositories;
using DataBalk.Task.Api.Infrastructure.MediatR;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DataBalk.Task.Api.Features.Tasks.Queries;

public class GetTaskQuery: AppMessage, IRequest<Result<Data.Entities.Task>>
{
    public long Id { get; set; }
}

public class GetTaskQueryValidator : AbstractValidator<GetTaskQuery>
{
    public GetTaskQueryValidator()
    {
        RuleFor(qry => qry.Id).NotEmpty().WithMessage(TaskConstants.ValidationMessages.RequiredField(nameof(GetTaskQuery.Id)));
    }
}

public class GetTaskQueryHandler: IRequestHandler<GetTaskQuery, Result<Data.Entities.Task>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<GetTaskQuery> _logger;

    public GetTaskQueryHandler(ITaskRepository taskRepository, ILogger<GetTaskQuery> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task<Result<Data.Entities.Task>> Handle(GetTaskQuery qry, CancellationToken ctx)
    {
        _logger.LogInformation($"{nameof(GetTaskQuery)} start");

        var task = await _taskRepository.GetTaskByIdAsync(qry.Id, ctx);

        return new SuccessResult<Data.Entities.Task>(task);
    }
}
