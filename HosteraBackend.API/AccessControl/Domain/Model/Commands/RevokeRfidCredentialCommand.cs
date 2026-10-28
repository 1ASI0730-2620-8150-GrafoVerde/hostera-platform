namespace HosteraBackend.API.AccessControl.Domain.Model.Commands;

/// <summary>
///     Command to revoke an active RFID credential
/// </summary>
/// <param name="CredentialId">The identifier of the credential to revoke</param>
/// <param name="Reason">The reason for the revocation, such as a lost card</param>
public record RevokeRfidCredentialCommand(int CredentialId, string Reason);
