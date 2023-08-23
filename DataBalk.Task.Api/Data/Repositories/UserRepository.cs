using DataBalk.Task.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataBalk.Task.Api.Data.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken ctx);
    Task<User> GetUserByIdAsync(long id, CancellationToken ctx);
    System.Threading.Tasks.Task CreateUserAsync(User user, CancellationToken ctx);
    System.Threading.Tasks.Task UpdateUserAsync(User user, CancellationToken ctx);
    System.Threading.Tasks.Task DeleteUserAsync(long id, CancellationToken ctx);
    Task<bool> IsUserValidAsync(string username, string password, CancellationToken ctx);
    Task<bool> UsernameAlreadyExistsAsync(string username, CancellationToken ctx);
}
public class UserRepository: IUserRepository
{
    private readonly TaskDbContext _taskDbContext;

    public UserRepository(TaskDbContext taskDbContext)
    {
        _taskDbContext = taskDbContext;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken ctx)
    {
        return await _taskDbContext.Users.ToListAsync(ctx);
    }

    public async Task<User> GetUserByIdAsync(long id, CancellationToken ctx)
    {
        return await _taskDbContext.Users.FindAsync(id, ctx);
    }

    public async System.Threading.Tasks.Task CreateUserAsync(User user, CancellationToken ctx)
    {
        _taskDbContext.Users.Add(user);
        await _taskDbContext.SaveChangesAsync(ctx);
    }

    public async System.Threading.Tasks.Task UpdateUserAsync(User user, CancellationToken ctx)
    {
        _taskDbContext.Entry(user).State = EntityState.Modified;
        await _taskDbContext.SaveChangesAsync(ctx);
    }

    public async System.Threading.Tasks.Task DeleteUserAsync(long id, CancellationToken ctx)
    {
        var user = await GetUserByIdAsync(id, ctx);
        _taskDbContext.Users.Remove(user);
        await _taskDbContext.SaveChangesAsync(ctx);
    }

    public async Task<bool> IsUserValidAsync(string username, string password, CancellationToken ctx)
    {
        var user = await _taskDbContext.Users
            .FirstOrDefaultAsync(u => u.Username == username.ToLower() &&
                                      u.Password == password, ctx);

        return user != null;
    }

    public async Task<bool> UsernameAlreadyExistsAsync(string username, CancellationToken ctx)
    {
        var userExists = await _taskDbContext.Users
            .AnyAsync(u => u.Username == username.ToLower(), ctx);

        return userExists;
    }
}
