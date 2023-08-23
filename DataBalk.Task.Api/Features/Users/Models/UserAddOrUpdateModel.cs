namespace DataBalk.Task.Api.Features.Users.Models;

public class UserAddOrUpdateModel
{
    public long Id { get; set; } = default;
    public string Username { get; set; } = string.Empty;
    public string EMail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
