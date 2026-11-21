namespace HosteraBackend.API.Iam.Domain.Model.Aggregates;

/// <summary>
///     Represents a user who signs in to operate the Hostera platform.
/// </summary>
/// <remarks>
///     The user keeps only the hash of the password, never the password itself.
/// </remarks>
public partial class User
{
    /// <summary>
    ///     The maximum number of characters of a username.
    /// </summary>
    public const int UsernameMaxLength = 50;

    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected User()
    {
        Username = null!;
        PasswordHash = null!;
    }

    /// <summary>
    ///     Creates a user with a username and the hash of the password.
    /// </summary>
    /// <param name="username">The username. Surrounding spaces are removed.</param>
    /// <param name="passwordHash">The hash of the password.</param>
    /// <exception cref="ArgumentException">Thrown when the username or the password hash is empty, or the username is too long.</exception>
    public User(string username, string passwordHash)
    {
        Username = NormalizeUsername(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        PasswordHash = passwordHash;
    }

    /// <summary>
    ///     Gets the server-generated identifier for this user.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the username the user signs in with.
    /// </summary>
    public string Username { get; private set; }

    /// <summary>
    ///     Gets the hash of the password of the user.
    /// </summary>
    public string PasswordHash { get; private set; }

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

    private static string NormalizeUsername(string username)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        var normalized = username.Trim();
        if (normalized.Length > UsernameMaxLength)
            throw new ArgumentException($"The username cannot be longer than {UsernameMaxLength} characters.",
                nameof(username));
        return normalized;
    }
}
