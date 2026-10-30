using HosteraBackend.API.Overview.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Overview.Domain.Model.ReadModels;

/// <summary>
///     Status period of a room, as the Overview bounded context reads it from the Rooms bounded context
/// </summary>
/// <param name="RoomId">The identifier of the room</param>
/// <param name="Status">The status of the room during the period</param>
/// <param name="StartDate">The first day of the period</param>
/// <param name="EndDate">The last day of the period, included</param>
public record StatusPeriodReadModel(int RoomId, ERoomStatus Status, DateOnly StartDate, DateOnly EndDate)
{
    /// <summary>
    ///     Indicates whether the period includes a day.
    /// </summary>
    /// <param name="date">The day to evaluate.</param>
    /// <returns>True when the day is between the first and the last day of the period.</returns>
    public bool Covers(DateOnly date) => StartDate <= date && date <= EndDate;
}
