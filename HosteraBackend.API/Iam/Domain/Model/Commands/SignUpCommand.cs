using HosteraBackend.API.Iam.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Iam.Domain.Model.Commands;

/// <summary>
///     Command to sign up a new user
/// </summary>
/// <param name="Username">The username of the new user</param>
/// <param name="Password">The password of the new user</param>
/// <param name="FullName">The full name of the person who uses the account</param>
/// <param name="OrganizationId">The identifier of the organization of the user, or null to start a new organization</param>
/// <param name="Role">
///     The role of the user in an existing organization. A user who starts a new organization is always an
///     <see cref="EUserRole.OperationsManager" />
/// </param>
public record SignUpCommand(
    string Username,
    string Password,
    string FullName,
    int? OrganizationId = null,
    EUserRole Role = EUserRole.OperationsManager)
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
