namespace HosteraBackend.API.Rooms.Domain.Model.ValueObjects;

/// <summary>
///     The status of a room on one day.
/// </summary>
/// <param name="Date">The day.</param>
/// <param name="Status">The EDayStatus of the room on the day.</param>
public record RoomDayStatus(DateOnly Date, EDayStatus Status);
