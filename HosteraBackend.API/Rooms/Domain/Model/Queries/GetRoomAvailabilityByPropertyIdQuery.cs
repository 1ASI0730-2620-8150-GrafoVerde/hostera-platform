using HosteraBackend.API.Rooms.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Rooms.Domain.Model.Queries;

/// <summary>
///     Query to get the day statuses of the rooms of a property for a week
/// </summary>
/// <param name="PropertyId">The property ID to search</param>
/// <param name="StartDate">The first day of the week</param>
/// <param name="Status">Keep only the rooms with this EDayStatus on the first day, or null for all rooms</param>
public record GetRoomAvailabilityByPropertyIdQuery(int PropertyId, DateOnly StartDate, EDayStatus? Status = null)
{
    /// <summary>
    ///     The number of days returned for each room.
    /// </summary>
    public const int Days = 7;
}
