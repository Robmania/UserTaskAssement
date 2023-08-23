namespace DataBalk.Task.Api.Features.Users.Models;

/// <summary>
/// Represents the data required to add or update a user.
/// </summary>
public class UserAddOrUpdateModel
{
    /// <summary>
    /// The unique identifier of the user. Use the 0 when adding a new user.
    /// </summary>
    public long Id { get; set; } = default;

    /// <summary>
    /// The username for the user. Must be unique across all users.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The email address for the user. Must be in a valid email format.
    /// </summary>
    public string EMail { get; set; } = string.Empty;

    /// <summary>
    /// The password for the user. It should meet the application's password strength requirements.
    /// Your password length must be at least 8.
    /// Your password length must not exceed 16.
    /// Your password must contain at least one uppercase letter.
    /// Your password must contain at least one lowercase letter.
    /// Your password must contain at least one (!@#$%^&*)
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

