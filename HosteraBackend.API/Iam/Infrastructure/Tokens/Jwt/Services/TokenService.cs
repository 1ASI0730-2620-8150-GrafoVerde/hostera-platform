using System.Globalization;
using System.Security.Claims;
using System.Text;
using HosteraBackend.API.Iam.Application.Internal.OutboundServices;
using HosteraBackend.API.Iam.Domain.Model.Aggregates;
using HosteraBackend.API.Iam.Infrastructure.Tokens.Jwt.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace HosteraBackend.API.Iam.Infrastructure.Tokens.Jwt.Services;

/// <summary>
///     JSON Web Token implementation of <see cref="ITokenService" />.
/// </summary>
/// <remarks>
///     Tokens are signed with HMAC-SHA256 using the secret of <see cref="TokenSettings" />, carry the identifier and
///     username of the user, and expire seven days after they are issued.
/// </remarks>
/// <param name="tokenSettings">The <see cref="TokenSettings" /> with the signing secret.</param>
/// <param name="logger">The logger that records why a token was rejected.</param>
public class TokenService(IOptions<TokenSettings> tokenSettings, ILogger<TokenService> logger) : ITokenService
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromDays(7);

    private readonly TokenSettings _tokenSettings = tokenSettings.Value;

    /// <inheritdoc />
    public string GenerateToken(User user)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Sid, user.Id.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Name, user.Username)
            ]),
            Expires = DateTime.UtcNow.Add(TokenLifetime),
            SigningCredentials = new SigningCredentials(GetSigningKey(), SecurityAlgorithms.HmacSha256Signature)
        };
        return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
    }

    /// <inheritdoc />
    public async Task<int?> ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        var tokenValidationResult = await new JsonWebTokenHandler().ValidateTokenAsync(token,
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = GetSigningKey(),
                ValidateIssuer = false,
                ValidateAudience = false,
                // Expiration without delay
                ClockSkew = TimeSpan.Zero
            });

        if (!tokenValidationResult.IsValid)
        {
            logger.LogDebug(tokenValidationResult.Exception, "A bearer token was rejected.");
            return null;
        }

        var userIdClaim = tokenValidationResult.ClaimsIdentity.FindFirst(ClaimTypes.Sid)?.Value;
        return int.TryParse(userIdClaim, NumberStyles.None, CultureInfo.InvariantCulture, out var userId)
            ? userId
            : null;
    }

    private SymmetricSecurityKey GetSigningKey() => new(Encoding.UTF8.GetBytes(_tokenSettings.Secret));
}
