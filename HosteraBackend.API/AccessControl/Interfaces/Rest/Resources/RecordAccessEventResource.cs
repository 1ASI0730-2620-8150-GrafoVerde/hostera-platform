using System.ComponentModel.DataAnnotations;

namespace HosteraBackend.API.AccessControl.Interfaces.Rest.Resources;

/// <summary>
///     Record access event resource for REST API
/// </summary>
/// <param name="PropertyId">The identifier of the property of the reader</param>
/// <param name="CardId">The identifier read from the card</param>
/// <param name="AccessPoint">The access point of the reader: main-entrance, room-door, elevator, service-corridor or storage-room</param>
/// <param name="RoomId">The identifier of the room of a room door reader</param>
/// <param name="OccurredAt">The moment the reader received the card, or null for the moment of the request</param>
public record RecordAccessEventResource(
    [Range(1, int.MaxValue)] int PropertyId,
    [Required] string CardId,
    [Required] string AccessPoint,
    [Range(1, int.MaxValue)] int? RoomId,
    DateTimeOffset? OccurredAt);
