namespace DataBalk.Task.Api.Features.Tasks.Models;

/// <summary>
/// Represents the data required to add or update a task.
/// </summary>
public class TaskAddOrUpdateModel
{
    /// <summary>
    /// The unique identifier of the task. Use 0 for a new task.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The title of the task.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// A description providing more details about the task.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The unique identifier of the user assigned to the task.
    /// </summary>
    public long Assignee { get; set; }

    /// <summary>
    /// The date and time by which the task should be completed, optional field and can be NULL
    /// </summary>
    public DateTime? DueDate { get; set; }
}

