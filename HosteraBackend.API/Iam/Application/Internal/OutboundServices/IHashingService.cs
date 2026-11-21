namespace HosteraBackend.API.Iam.Application.Internal.OutboundServices;

/// <summary>
///     Hashes passwords and verifies them against their hash.
/// </summary>
public interface IHashingService
{
    /// <summary>
    ///     Hashes a password.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>The hash of the password.</returns>
    string HashPassword(string password);

    /// <summary>
    ///     Verifies a password against a hash.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="passwordHash">The hash to verify the password against.</param>
    /// <returns>True if the password matches the hash, false otherwise.</returns>
    bool VerifyPassword(string password, string passwordHash);
}
