using System.ComponentModel.DataAnnotations;

namespace HosteraBackend.API.AccessControl.Interfaces.Rest.Resources;

/// <summary>
///     Revoke credential resource for REST API
/// </summary>
/// <param name="Reason">The reason: lost-card, damaged-card, security-risk, staff-left, replaced or other</param>
/// <param name="Note">The internal note, required for the other reason</param>
/// <param name="OperatorId">The identifier of the operator who revokes the credential</param>
public record RevokeCredentialResource(
    [Required] string Reason,
    [MaxLength(240)] string? Note,
    [Range(1, int.MaxValue)] int OperatorId);
