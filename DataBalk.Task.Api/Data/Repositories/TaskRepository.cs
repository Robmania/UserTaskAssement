using Microsoft.EntityFrameworkCore;

namespace DataBalk.Task.Api.Data.Repositories;

public interface ITaskRepository
{
    Task<IEnumerable<Entities.Task>> GetAllTasksAsync(CancellationToken ctx);
    Task<Entities.Task> GetTaskByIdAsync(long id, CancellationToken ctx);
    System.Threading.Tasks.Task CreateTaskAsync(Entities.Task task, CancellationToken ctx);
    System.Threading.Tasks.Task UpdateTaskAsync(Entities.Task task, CancellationToken ctx);
    System.Threading.Tasks.Task DeleteTaskAsync(long id, CancellationToken ctx);
}

public class TaskRepository: ITaskRepository
{
    private readonly TaskDbContext _taskDbContext;

    public TaskRepository(TaskDbContext taskDbContext)
    {
        _taskDbContext = taskDbContext;
    }

    public async Task<IEnumerable<Entities.Task>> GetAllTasksAsync(CancellationToken ctx)
    {
        return await _taskDbContext.Tasks.ToListAsync(ctx);
    }

    public async Task<Entities.Task> GetTaskByIdAsync(long id, CancellationToken ctx)
    {
        return await _taskDbContext.Tasks.FindAsync(id, ctx);
    }

    public async System.Threading.Tasks.Task CreateTaskAsync(Entities.Task task, CancellationToken ctx)
    {
        _taskDbContext.Tasks.Add(task);
        await _taskDbContext.SaveChangesAsync(ctx);
    }

    public async System.Threading.Tasks.Task UpdateTaskAsync(Entities.Task task, CancellationToken ctx)
    {
        _taskDbContext.Entry(task).State = EntityState.Modified;
        await _taskDbContext.SaveChangesAsync(ctx);
    }

    public async System.Threading.Tasks.Task DeleteTaskAsync(long id, CancellationToken ctx)
    {
        var task = await GetTaskByIdAsync(id, ctx);
        _taskDbContext.Tasks.Remove(task);
        await _taskDbContext.SaveChangesAsync(ctx);
    }
}
