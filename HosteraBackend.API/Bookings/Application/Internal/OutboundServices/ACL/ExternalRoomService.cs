using HosteraBackend.API.Bookings.Domain.Model.ReadModels;
using HosteraBackend.API.Rooms.Interfaces.ACL;

namespace HosteraBackend.API.Bookings.Application.Internal.OutboundServices.ACL;

/// <summary>
///     Anti-corruption service that reads rooms, room types, rate plans and prices from the Rooms bounded context.
/// </summary>
/// <param name="roomsContextFacade">The facade of the Rooms bounded context.</param>
public class ExternalRoomService(IRoomsContextFacade roomsContextFacade)
{
    /// <summary>
    ///     Fetches a room that a booking can hold.
    /// </summary>
    /// <param name="roomId">The identifier of the room.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The room, or null when it does not exist.</returns>
    public async Task<BookableRoom?> FetchRoomAsync(int roomId, CancellationToken cancellationToken)
    {
        var room = await roomsContextFacade.FetchRoomAsync(roomId, cancellationToken);
        return room is null ? null : new BookableRoom(room.Id, room.PropertyId, room.RoomTypeId);
    }

    /// <summary>
    ///     Fetches a room type that a booking can use.
    /// </summary>
    /// <param name="roomTypeId">The identifier of the room type.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The room type, or null when it does not exist.</returns>
    public async Task<BookableRoomType?> FetchRoomTypeAsync(int roomTypeId, CancellationToken cancellationToken)
    {
        var roomType = await roomsContextFacade.FetchRoomTypeAsync(roomTypeId, cancellationToken);
        return roomType is null
            ? null
            : new BookableRoomType(roomType.Id, roomType.PropertyId, roomType.Capacity, roomType.IsActive);
    }

    /// <summary>
    ///     Fetches a rate plan that can price a booking.
    /// </summary>
    /// <param name="ratePlanId">The identifier of the rate plan.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The rate plan, or null when it does not exist.</returns>
    public async Task<BookableRatePlan?> FetchRatePlanAsync(int ratePlanId, CancellationToken cancellationToken)
    {
        var ratePlan = await roomsContextFacade.FetchRatePlanAsync(ratePlanId, cancellationToken);
        return ratePlan is null
            ? null
            : new BookableRatePlan(ratePlan.Id, ratePlan.PropertyId, ratePlan.IsActive,
                ratePlan.AppliesToAllRoomTypes ? null : ratePlan.RoomTypeIds);
    }

    /// <summary>
    ///     Calculates the price of all nights of a stay.
    /// </summary>
    /// <param name="roomTypeId">The identifier of the room type.</param>
    /// <param name="ratePlanId">The identifier of the rate plan.</param>
    /// <param name="checkInDate">The check-in day.</param>
    /// <param name="checkOutDate">The check-out day.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The price of the stay, or null when it cannot be calculated.</returns>
    public Task<decimal?> QuoteStayAsync(int roomTypeId, int ratePlanId, DateOnly checkInDate, DateOnly checkOutDate,
        CancellationToken cancellationToken) =>
        roomsContextFacade.QuoteStayAsync(roomTypeId, ratePlanId, checkInDate, checkOutDate, cancellationToken);

    /// <summary>
    ///     Indicates whether an operational status prevents booking a room on a night of a stay.
    /// </summary>
    /// <param name="roomId">The identifier of the room.</param>
    /// <param name="checkInDate">The check-in day.</param>
    /// <param name="checkOutDate">The check-out day.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True when a Blocked or Out of service status covers a night of the stay.</returns>
    public Task<bool> IsRoomOutOfOrderAsync(int roomId, DateOnly checkInDate, DateOnly checkOutDate,
        CancellationToken cancellationToken) =>
        roomsContextFacade.HasBookingBlockingStatusAsync(roomId, checkInDate, checkOutDate.AddDays(-1),
            cancellationToken);

    /// <summary>
    ///     Records that a room needs cleaning on the day a guest leaves it.
    /// </summary>
    /// <param name="roomId">The identifier of the room.</param>
    /// <param name="date">The day the guest left.</param>
    /// <param name="bookingCode">The code of the booking whose guest left, used as the reason.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True when the room was marked; false when the day already had an operational status.</returns>
    public Task<bool> MarkRoomForCleaningAsync(int roomId, DateOnly date, string bookingCode,
        CancellationToken cancellationToken) =>
        roomsContextFacade.MarkRoomForCleaningAsync(roomId, date, $"Check-out of {bookingCode}", cancellationToken);
}
