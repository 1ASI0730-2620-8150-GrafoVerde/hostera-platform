using HosteraBackend.API.Bookings.Domain.Model.Aggregates;
using HosteraBackend.API.Bookings.Domain.Model.Queries;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.Bookings.Domain.Repositories;

/// <summary>
///     The booking repository contract.
/// </summary>
public interface IBookingRepository : IBaseRepository<Booking>
{
    /// <summary>
    ///     Find the bookings of a property that satisfy the criteria of a query
    /// </summary>
    /// <param name="query">The query with the property, the criteria and the optional page.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     An Enumerable containing the matching bookings ordered by check-in day and code, or empty otherwise.
    /// </returns>
    Task<IEnumerable<Booking>> FindByPropertyIdAsync(GetAllBookingsByPropertyIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Count the bookings of a property that satisfy the criteria of a query, ignoring its page
    /// </summary>
    /// <param name="query">The query with the property and the criteria.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the number of matching bookings.</returns>
    Task<int> CountByPropertyIdAsync(GetAllBookingsByPropertyIdQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Find the highest code number used by the bookings of a property
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing the highest code number, or 0 when the property has no bookings.</returns>
    Task<int> FindLatestCodeNumberByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Indicates whether another Pending, Confirmed or Checked in booking holds a room on any night of a stay
    /// </summary>
    /// <param name="roomId">The identifier of the room.</param>
    /// <param name="checkInDate">The check-in day of the stay.</param>
    /// <param name="checkOutDate">The check-out day of the stay.</param>
    /// <param name="excludedBookingId">The identifier of a booking to ignore, or null to check them all.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task containing true when another booking holds the room on a night of the stay.</returns>
    Task<bool> ExistsRoomHoldAsync(int roomId, DateOnly checkInDate, DateOnly checkOutDate, int? excludedBookingId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Find the Pending, Confirmed and Checked in bookings that hold a room of a property on a night of a range
    /// </summary>
    /// <param name="query">The query with the property, the range of nights and the optional room.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>An Enumerable containing the bookings ordered by room and check-in day, or empty otherwise.</returns>
    Task<IEnumerable<Booking>> FindRoomHoldsAsync(GetRoomHoldsByPropertyIdQuery query,
        CancellationToken cancellationToken = default);
}
