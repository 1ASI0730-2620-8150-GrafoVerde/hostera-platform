using HosteraBackend.API.Overview.Domain.Model.ReadModels;

namespace HosteraBackend.API.Overview.Domain.Repositories;

/// <summary>
///     The read-only repository contract of the Overview bounded context.
/// </summary>
/// <remarks>
///     The Overview bounded context has no tables of its own: it reads the rooms, status periods and bookings that
///     the Rooms and Bookings bounded contexts store. Each method returns null when its source cannot provide data,
///     so the dashboard can still show the information of the other sources.
/// </remarks>
public interface IOverviewReadRepository
{
    /// <summary>
    ///     Find the rooms of a property
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     The rooms of the property, or null when the rooms cannot be read.
    /// </returns>
    Task<IReadOnlyList<RoomReadModel>?> FindRoomsByPropertyIdAsync(int propertyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Find the status periods of the rooms of a property that include a range of days
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="from">The first day of the range.</param>
    /// <param name="to">The last day of the range, included.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     The status periods that include at least one day of the range, or null when they cannot be read.
    /// </returns>
    Task<IReadOnlyList<StatusPeriodReadModel>?> FindStatusPeriodsByPropertyIdAsync(int propertyId, DateOnly from,
        DateOnly to, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Find the bookings of a property whose stay includes a range of nights
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="from">The first night of the range.</param>
    /// <param name="to">The last night of the range, included.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     The bookings that arrive before the end of the range and leave after its start, or null when they cannot
    ///     be read.
    /// </returns>
    Task<IReadOnlyList<BookingReadModel>?> FindBookingsByPropertyIdAsync(int propertyId, DateOnly from, DateOnly to,
        CancellationToken cancellationToken = default);
}
