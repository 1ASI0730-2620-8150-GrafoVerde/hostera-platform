namespace HosteraBackend.API.Iam.Domain.Model.Commands;

/// <summary>
///     Command to sign in a user
/// </summary>
/// <param name="Username">The username of the user</param>
/// <param name="Password">The password of the user</param>
public record SignInCommand(string Username, string Password);
