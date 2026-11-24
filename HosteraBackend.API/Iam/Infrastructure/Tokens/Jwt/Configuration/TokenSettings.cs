using System.Text;

namespace HosteraBackend.API.Iam.Infrastructure.Tokens.Jwt.Configuration;

/// <summary>
///     Settings of the JSON Web Tokens, read from the <c>TokenSettings</c> configuration section.
/// </summary>
/// <remarks>
///     Keep the secret out of the repository in deployed environments: set it with the
///     <c>TokenSettings__Secret</c> environment variable.
/// </remarks>
public class TokenSettings
{
    /// <summary>
    ///     The name of the configuration section.
    /// </summary>
    public const string SectionName = "TokenSettings";

    /// <summary>
    ///     The minimum number of bytes of the secret, the size of an HMAC-SHA256 key.
    /// </summary>
    public const int SecretMinBytes = 32;

    /// <summary>
    ///     Gets or sets the secret that signs and validates the tokens.
    /// </summary>
    public required string Secret { get; set; }

    /// <summary>
    ///     Gets whether the secret is long enough to sign tokens with HMAC-SHA256.
    /// </summary>
    public bool HasValidSecret => !string.IsNullOrWhiteSpace(Secret) &&
                                  Encoding.UTF8.GetByteCount(Secret) >= SecretMinBytes;
}
