namespace HosteraBackend.API.AccessControl.Interfaces.Rest.Resources;

/// <summary>
///     Access event resource for REST API
/// </summary>
/// <param name="Id">The unique identifier of the access event</param>
/// <param name="PropertyId">The identifier of the property of the reader</param>
/// <param name="OccurredAt">The moment the reader received the card</param>
/// <param name="CredentialId">The identifier of the credential written to the presented card</param>
/// <param name="CardId">The identifier read from the card</param>
/// <param name="HolderName">The name of the person who holds the credential</param>
/// <param name="HolderType">Whether the holder is a guest or a staff member: guest or staff</param>
/// <param name="AccessPoint">The access point of the reader: main-entrance, room-door, elevator, service-corridor or storage-room</param>
/// <param name="RoomId">The identifier of the room of the door, or of the room the guest key card opens</param>
/// <param name="Result">Whether the door opened: granted or denied</param>
/// <param name="DenialReason">The reason the door stayed closed: outside-scope, not-active-yet, expired or revoked</param>
public record AccessEventResource(
    int Id,
    int PropertyId,
    DateTimeOffset OccurredAt,
    int? CredentialId,
    string CardId,
    string HolderName,
    string HolderType,
    string AccessPoint,
    int? RoomId,
    string Result,
    string? DenialReason);
