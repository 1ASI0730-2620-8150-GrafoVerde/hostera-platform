namespace HosteraBackend.API.Bookings.Interfaces.ACL;

/// <summary>
///     Facade that other bounded contexts use to read which bookings hold the rooms of a property.
/// </summary>
public interface IBookingsContextFacade
{
    /// <summary>
    ///     Fetches the Pending, Confirmed and Checked in bookings that hold a room of a property on a night of a range.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="from">The first night of the range.</param>
    /// <param name="to">The last night of the range.</param>
    /// <param name="roomId">The identifier of a room to keep, or null for every room.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The room holds that share at least one night with the range.</returns>
    Task<IReadOnlyList<RoomHoldSummary>> FetchRoomHoldsAsync(int propertyId, DateOnly from, DateOnly to, int? roomId,
        CancellationToken cancellationToken);
}
