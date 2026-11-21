namespace HosteraBackend.API.Iam.Domain.Model.Commands;

/// <summary>
///     Command to sign up a new user
/// </summary>
/// <param name="Username">The username of the new user</param>
/// <param name="Password">The password of the new user</param>
public record SignUpCommand(string Username, string Password)
{
    /// <summary>
    ///     The minimum number of characters of a password.
    /// </summary>
    public const int PasswordMinLength = 8;

    /// <summary>
    ///     The maximum number of characters of a password. BCrypt only reads the first 72 bytes of a password.
    /// </summary>
    public const int PasswordMaxLength = 64;
}
