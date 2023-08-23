using DataBalk.Task.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataBalk.Task.Api.Data;

public class TaskDbContext: DbContext
{
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Entities.Task> Tasks { get; set; }

    public TaskDbContext(DbContextOptions<TaskDbContext> options)
        : base(options)
    {
    }
}
