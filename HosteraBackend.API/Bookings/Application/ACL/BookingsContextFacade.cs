using HosteraBackend.API.Bookings.Application.QueryServices;
using HosteraBackend.API.Bookings.Domain.Model.Queries;
using HosteraBackend.API.Bookings.Domain.Model.ValueObjects;
using HosteraBackend.API.Bookings.Interfaces.ACL;

namespace HosteraBackend.API.Bookings.Application.ACL;

/// <summary>
///     Facade that other bounded contexts use to read which bookings hold the rooms of a property.
/// </summary>
/// <param name="bookingQueryService">The booking query service</param>
public class BookingsContextFacade(IBookingQueryService bookingQueryService) : IBookingsContextFacade
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<RoomHoldSummary>> FetchRoomHoldsAsync(int propertyId, DateOnly from,
        DateOnly to, int? roomId, CancellationToken cancellationToken)
    {
        var bookings = await bookingQueryService.Handle(
            new GetRoomHoldsByPropertyIdQuery(propertyId, from, to, roomId), cancellationToken);
        return bookings
            .Select(booking => new RoomHoldSummary(booking.Id, booking.Code, booking.RoomId, booking.CheckInDate,
                booking.CheckOutDate, booking.Status == EBookingStatus.CheckedIn))
            .ToList();
    }
}
