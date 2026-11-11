using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.Rooms.Domain.Repositories;

/// <summary>
///     The status period repository contract.
/// </summary>
public interface IStatusPeriodRepository : IBaseRepository<StatusPeriod>
{
    /// <summary>
    ///     Find the status periods of the rooms of a property that overlap a date range
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="from">The first day of the range, or null for no lower limit.</param>
    /// <param name="to">The last day of the range, or null for no upper limit.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     An Enumerable containing the status periods ordered by room and start date, or empty otherwise.
    /// </returns>
    Task<IEnumerable<StatusPeriod>> FindByPropertyIdAsync(int propertyId, DateOnly? from, DateOnly? to,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Find the status periods of a room that overlap a date range
    /// </summary>
    /// <param name="roomId">The identifier of the room.</param>
    /// <param name="startDate">The first day of the range.</param>
    /// <param name="endDate">The last day of the range.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     An Enumerable containing the overlapping status periods, or empty otherwise.
    /// </returns>
    Task<IEnumerable<StatusPeriod>> FindByRoomIdAndPeriodAsync(int roomId, DateOnly startDate, DateOnly endDate,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Find every status period of a room
    /// </summary>
    /// <param name="roomId">The identifier of the room.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     An Enumerable containing the status periods of the room ordered by start date, or empty otherwise.
    /// </returns>
    Task<IEnumerable<StatusPeriod>> FindByRoomIdAsync(int roomId, CancellationToken cancellationToken = default);
}
