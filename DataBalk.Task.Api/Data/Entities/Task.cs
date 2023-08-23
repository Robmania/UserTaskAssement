using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataBalk.Task.Api.Data.Entities;

[Table("Task")]
public class Task
{
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    [Column("Title")]
    public string Title { get; set; } = string.Empty;

    [Column("Description")]
    public string Description { get; set; } = string.Empty;

    [Column("Assignee")]
    public long Assignee { get; set; }
    
    [Column("DueDate")]
    public DateTime? DueDate { get; set; }
}
