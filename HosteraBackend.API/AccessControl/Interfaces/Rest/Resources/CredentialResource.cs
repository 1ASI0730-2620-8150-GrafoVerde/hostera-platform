namespace HosteraBackend.API.AccessControl.Interfaces.Rest.Resources;

/// <summary>
///     Credential resource for REST API
/// </summary>
/// <param name="Id">The unique identifier of the credential</param>
/// <param name="PropertyId">The identifier of the property that owns the credential</param>
/// <param name="CardId">The identifier written to the card</param>
/// <param name="Type">The kind of credential: guest-key-card or staff-credential</param>
/// <param name="Status">The current status: scheduled, active, expired or revoked</param>
/// <param name="HolderName">The name of the person who holds the credential</param>
/// <param name="BookingId">The identifier of the booking of a guest key card</param>
/// <param name="BookingCode">The code of the booking of a guest key card</param>
/// <param name="RoomId">The identifier of the room a guest key card opens</param>
/// <param name="StaffMemberId">The identifier of the staff member who holds a staff credential</param>
/// <param name="Scope">The areas a staff credential opens</param>
/// <param name="ValidFrom">The moment the access starts</param>
/// <param name="ValidUntil">The moment the access ends, or null for permanent access</param>
/// <param name="IssuedAt">The moment the credential was issued</param>
/// <param name="IssuedBy">The identifier of the operator who issued the credential</param>
/// <param name="RevokedAt">The moment the credential was revoked</param>
/// <param name="RevokedBy">The identifier of the operator who revoked the credential</param>
/// <param name="RevocationReason">The reason for the revocation</param>
/// <param name="RevocationNote">The internal note about the revocation</param>
public record CredentialResource(
    int Id,
    int PropertyId,
    string CardId,
    string Type,
    string Status,
    string HolderName,
    int? BookingId,
    string? BookingCode,
    int? RoomId,
    int? StaffMemberId,
    string? Scope,
    DateTimeOffset ValidFrom,
    DateTimeOffset? ValidUntil,
    DateTimeOffset IssuedAt,
    int IssuedBy,
    DateTimeOffset? RevokedAt,
    int? RevokedBy,
    string? RevocationReason,
    string? RevocationNote);
