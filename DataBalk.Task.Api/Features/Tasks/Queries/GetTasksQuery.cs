using DataBalk.Task.Api.Data.Repositories;
using DataBalk.Task.Api.Infrastructure.MediatR;
using MediatR;

namespace DataBalk.Task.Api.Features.Tasks.Queries;

public class GetTasksQuery : AppMessage, IRequest<Result<List<Data.Entities.Task>>>
{
}

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, Result<List<Data.Entities.Task>>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<GetTasksQuery> _logger;

    public GetTasksQueryHandler(ILogger<GetTasksQuery> logger, ITaskRepository taskRepository)
    {
        _logger = logger;
        _taskRepository = taskRepository;
    }

    public async Task<Result<List<Data.Entities.Task>>> Handle(GetTasksQuery qry, CancellationToken ctx)
    {
        _logger.LogInformation($"{nameof(GetTasksQuery)} start");

        var tasks = await _taskRepository.GetAllTasksAsync(ctx);

        return new SuccessResult<List<Data.Entities.Task>>(tasks.OrderBy(t=> t.DueDate).ToList());
    }
}