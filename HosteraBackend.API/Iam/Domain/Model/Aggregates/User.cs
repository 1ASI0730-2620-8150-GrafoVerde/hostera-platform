using HosteraBackend.API.Iam.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Iam.Domain.Model.Aggregates;

/// <summary>
///     Represents a user who signs in to operate the properties of an organization on the Hostera platform.
/// </summary>
/// <remarks>
///     The user keeps only the hash of the password, never the password itself. The full name and the role describe the
///     user to the people of the organization; the role does not change what the user can do.
/// </remarks>
public partial class User
{
    /// <summary>
    ///     The maximum number of characters of a username.
    /// </summary>
    public const int UsernameMaxLength = 50;

    /// <summary>
    ///     The minimum number of characters of a full name.
    /// </summary>
    public const int FullNameMinLength = 2;

    /// <summary>
    ///     The maximum number of characters of a full name.
    /// </summary>
    public const int FullNameMaxLength = 100;

    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected User()
    {
        Username = null!;
        PasswordHash = null!;
        FullName = null!;
    }

    /// <summary>
    ///     Creates a user of an organization with a username, the hash of the password, a full name, and a role.
    /// </summary>
    /// <param name="username">The username. Surrounding spaces are removed.</param>
    /// <param name="passwordHash">The hash of the password.</param>
    /// <param name="fullName">The full name of the person who uses the account. Surrounding spaces are removed.</param>
    /// <param name="role">The role of the user in the organization.</param>
    /// <param name="organizationId">The identifier of the organization of the user.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown when the username or the password hash is empty, the username is too long, the full name is too short
    ///     or too long, the role is not defined, or the organization identifier is not positive.
    /// </exception>
    public User(string username, string passwordHash, string fullName, EUserRole role, int organizationId)
    {
        Username = NormalizeUsername(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(organizationId);
        PasswordHash = passwordHash;
        FullName = NormalizeFullName(fullName);
        Role = ValidateRole(role);
        OrganizationId = organizationId;
    }

    /// <summary>
    ///     Gets the server-generated identifier for this user.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the identifier of the organization whose properties the user operates.
    /// </summary>
    public int OrganizationId { get; private set; }

    /// <summary>
    ///     Gets the username the user signs in with.
    /// </summary>
    public string Username { get; private set; }

    /// <summary>
    ///     Gets the hash of the password of the user.
    /// </summary>
    public string PasswordHash { get; private set; }

    /// <summary>
    ///     Gets the full name of the person who uses the account, such as "Lucía Martín".
    /// </summary>
    public string FullName { get; private set; }

    /// <summary>
    ///     Gets the role of the user in the organization. It is descriptive only and grants no permissions.
    /// </summary>
    public EUserRole Role { get; private set; }

    /// <summary>
    ///     Changes the username.
    /// </summary>
    /// <param name="username">The new username. Surrounding spaces are removed.</param>
    /// <returns>The updated user.</returns>
    /// <exception cref="ArgumentException">Thrown when the username is empty or too long.</exception>
    public User UpdateUsername(string username)
    {
        Username = NormalizeUsername(username);
        return this;
    }

    /// <summary>
    ///     Changes the hash of the password.
    /// </summary>
    /// <param name="passwordHash">The new hash of the password.</param>
    /// <returns>The updated user.</returns>
    /// <exception cref="ArgumentException">Thrown when the password hash is empty.</exception>
    public User UpdatePasswordHash(string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        PasswordHash = passwordHash;
        return this;
    }

    /// <summary>
    ///     Changes the full name and the role of the user.
    /// </summary>
    /// <param name="fullName">The new full name. Surrounding spaces are removed.</param>
    /// <param name="role">The new role.</param>
    /// <returns>The updated user.</returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when the full name is too short or too long, or the role is not defined. The user is left unchanged.
    /// </exception>
    public User UpdateProfile(string fullName, EUserRole role)
    {
        var normalizedFullName = NormalizeFullName(fullName);
        Role = ValidateRole(role);
        FullName = normalizedFullName;
        return this;
    }

    private static string NormalizeUsername(string username)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        var normalized = username.Trim();
        if (normalized.Length > UsernameMaxLength)
            throw new ArgumentException($"The username cannot be longer than {UsernameMaxLength} characters.",
                nameof(username));
        return normalized;
    }

    private static string NormalizeFullName(string fullName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        var normalized = fullName.Trim();
        if (normalized.Length is < FullNameMinLength or > FullNameMaxLength)
            throw new ArgumentException(
                $"The full name must have between {FullNameMinLength} and {FullNameMaxLength} characters.",
                nameof(fullName));
        return normalized;
    }

    private static EUserRole ValidateRole(EUserRole role)
    {
        if (!Enum.IsDefined(role))
            throw new ArgumentException("The role is not defined.", nameof(role));
        return role;
    }
}
