using HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

namespace HosteraBackend.API.AccessControl.Domain.Model.Commands;

/// <summary>
///     Command to revoke a credential
/// </summary>
/// <param name="CredentialId">The identifier of the credential to revoke</param>
/// <param name="Reason">The ERevocationReason of the revocation</param>
/// <param name="Note">The internal note, required for the Other reason</param>
/// <param name="OperatorId">The identifier of the operator who revokes the credential</param>
public record RevokeCredentialCommand(int CredentialId, ERevocationReason Reason, string? Note, int OperatorId);
