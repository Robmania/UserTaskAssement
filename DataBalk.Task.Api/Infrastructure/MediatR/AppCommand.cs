using DataBalk.Task.Api.Enums;

namespace DataBalk.Task.Api.Infrastructure.MediatR;

public abstract class AppCommand : AppMessage
{
    // Most command classes will require an Id property
    // representing the unique database Id of the record being added or updated
    public long Id { get; set; } = default;
    public EOperation Operation { get; set; }

    public abstract string CommandDescription { get; }
}
