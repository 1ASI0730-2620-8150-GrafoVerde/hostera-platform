namespace HosteraBackend.API.Iam.Domain.Model.Commands;

/// <summary>
///     Command to sign up a new user
/// </summary>
/// <param name="Username">The username of the new user</param>
/// <param name="Password">The password of the new user</param>
public record SignUpCommand(string Username, string Password);
