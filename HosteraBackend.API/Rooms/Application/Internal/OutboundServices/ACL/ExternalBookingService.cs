using HosteraBackend.API.Bookings.Interfaces.ACL;
using HosteraBackend.API.Rooms.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Rooms.Application.Internal.OutboundServices.ACL;

/// <summary>
///     Anti-corruption service that reads the room assignments of a property from the Bookings bounded context.
/// </summary>
/// <param name="bookingsContextFacade">The facade of the Bookings bounded context.</param>
public class ExternalBookingService(IBookingsContextFacade bookingsContextFacade)
{
    /// <summary>
    ///     Fetches the bookings that hold a room of a property on a night of a range.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="from">The first night of the range.</param>
    /// <param name="to">The last night of the range.</param>
    /// <param name="roomId">The identifier of a room to keep, or null for every room.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The room assignments that share at least one night with the range.</returns>
    public async Task<IReadOnlyList<RoomAssignment>> FetchRoomAssignmentsAsync(int propertyId, DateOnly from,
        DateOnly to, int? roomId, CancellationToken cancellationToken)
    {
        var roomHolds = await bookingsContextFacade.FetchRoomHoldsAsync(propertyId, from, to, roomId,
            cancellationToken);
        return roomHolds
            .Select(hold => new RoomAssignment(hold.RoomId, hold.BookingCode, hold.CheckInDate, hold.CheckOutDate,
                hold.IsCheckedIn))
            .ToList();
    }
}
