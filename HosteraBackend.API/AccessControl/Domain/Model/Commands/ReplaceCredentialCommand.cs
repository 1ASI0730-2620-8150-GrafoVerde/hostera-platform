using HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

namespace HosteraBackend.API.AccessControl.Domain.Model.Commands;

/// <summary>
///     Command to replace a credential with a new card
/// </summary>
/// <param name="CredentialId">The identifier of the credential to replace</param>
/// <param name="CardId">The CardId of the new card</param>
/// <param name="OperatorId">The identifier of the operator who replaces the credential</param>
public record ReplaceCredentialCommand(int CredentialId, CardId CardId, int OperatorId);
