namespace DataBalk.Task.Api.Features.Tasks.Models;

public class TaskAddOrUpdateModel
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long Assignee { get; set; }
    public DateTime? DueDate { get; set; }
}
