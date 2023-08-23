using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataBalk.Task.Api.Data.Entities;

[Table("User")]
public class User
{
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    [Column("Username")]
    public string Username { get; set; } = string.Empty;

    [Column("Email")]
    public string EMail { get; set; } = string.Empty;

    [Column("Password")]
    public string Password { get; set; } = string.Empty;
}
