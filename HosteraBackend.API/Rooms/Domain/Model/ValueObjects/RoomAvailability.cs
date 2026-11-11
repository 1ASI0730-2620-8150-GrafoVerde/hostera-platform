using HosteraBackend.API.Rooms.Domain.Model.Aggregates;

namespace HosteraBackend.API.Rooms.Domain.Model.ValueObjects;

/// <summary>
///     The day statuses of a room over consecutive days.
/// </summary>
/// <param name="Room">The room.</param>
/// <param name="Days">The status of the room on each day, in date order.</param>
public record RoomAvailability(Room Room, IReadOnlyList<RoomDayStatus> Days);
