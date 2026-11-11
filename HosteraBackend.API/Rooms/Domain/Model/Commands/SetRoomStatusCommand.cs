using HosteraBackend.API.Rooms.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Rooms.Domain.Model.Commands;

/// <summary>
///     Command to set or release the operational status of a room over a date range
/// </summary>
/// <param name="RoomId">The identifier of the room</param>
/// <param name="Status">The EOperationalStatus to set, or null to make the days available again</param>
/// <param name="StartDate">The first day of the range</param>
/// <param name="EndDate">The last day of the range</param>
/// <param name="Reason">Why the status is set; required unless the days become available</param>
public record SetRoomStatusCommand(
    int RoomId,
    EOperationalStatus? Status,
    DateOnly StartDate,
    DateOnly EndDate,
    string? Reason);
