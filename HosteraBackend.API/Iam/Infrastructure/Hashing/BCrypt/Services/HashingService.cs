using HosteraBackend.API.Iam.Application.Internal.OutboundServices;
using BCryptNet = BCrypt.Net.BCrypt;

namespace HosteraBackend.API.Iam.Infrastructure.Hashing.BCrypt.Services;

/// <summary>
///     BCrypt implementation of <see cref="IHashingService" />.
/// </summary>
/// <remarks>
///     Each hash carries its own random salt and work factor, so equal passwords get different hashes.
/// </remarks>
public class HashingService : IHashingService
{
    /// <inheritdoc />
    public string HashPassword(string password)
    {
        return BCryptNet.HashPassword(password);
    }

    /// <inheritdoc />
    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCryptNet.Verify(password, passwordHash);
    }
}
