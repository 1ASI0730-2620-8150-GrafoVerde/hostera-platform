using HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;

namespace HosteraBackend.API.AccessControl.Domain.Model.Commands;

/// <summary>
///     Command to record a credential presented at an RFID reader of a property
/// </summary>
/// <param name="PropertyId">The identifier of the property of the reader</param>
/// <param name="CardId">The identifier read from the card</param>
/// <param name="AccessPoint">The access point of the reader</param>
/// <param name="RoomId">The identifier of the room of a room door reader, or null for other access points</param>
/// <param name="OccurredAt">The moment the reader received the card</param>
public record RecordAccessEventCommand(
    int PropertyId,
    CardId CardId,
    EAccessPoint AccessPoint,
    int? RoomId,
    DateTimeOffset OccurredAt);
