using HosteraBackend.API.Iam.Domain.Model.Aggregates;

namespace HosteraBackend.API.Iam.Application.Internal.OutboundServices;

/// <summary>
///     Generates and validates the bearer tokens of signed-in users.
/// </summary>
public interface ITokenService
{
    /// <summary>
    ///     Generates a token for a user.
    /// </summary>
    /// <param name="user">The user to generate the token for.</param>
    /// <returns>The generated token.</returns>
    string GenerateToken(User user);

    /// <summary>
    ///     Validates a token.
    /// </summary>
    /// <param name="token">The token to validate.</param>
    /// <returns>The identifier of the user if the token is valid and has not expired, null otherwise.</returns>
    Task<int?> ValidateToken(string token);
}
