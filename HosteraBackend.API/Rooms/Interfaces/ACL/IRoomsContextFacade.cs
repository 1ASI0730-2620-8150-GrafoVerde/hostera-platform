namespace HosteraBackend.API.Rooms.Interfaces.ACL;

/// <summary>
///     Facade that other bounded contexts use to read the rooms, room types, rate plans and nightly rates of the Rooms
///     bounded context.
/// </summary>
public interface IRoomsContextFacade
{
    /// <summary>
    ///     Fetches a room.
    /// </summary>
    /// <param name="roomId">The identifier of the room.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The room, or null when it does not exist.</returns>
    Task<RoomSummary?> FetchRoomAsync(int roomId, CancellationToken cancellationToken);

    /// <summary>
    ///     Fetches a room type.
    /// </summary>
    /// <param name="roomTypeId">The identifier of the room type.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The room type, or null when it does not exist.</returns>
    Task<RoomTypeSummary?> FetchRoomTypeAsync(int roomTypeId, CancellationToken cancellationToken);

    /// <summary>
    ///     Fetches a rate plan.
    /// </summary>
    /// <param name="ratePlanId">The identifier of the rate plan.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The rate plan, or null when it does not exist.</returns>
    Task<RatePlanSummary?> FetchRatePlanAsync(int ratePlanId, CancellationToken cancellationToken);

    /// <summary>
    ///     Calculates the price of a stay: the sum of the nightly rates of the room type under the rate plan for each
    ///     night from the check-in day up to the day before check-out.
    /// </summary>
    /// <remarks>
    ///     A night uses its daily rate when the plan has one, and the base nightly rate of the room type otherwise.
    /// </remarks>
    /// <param name="roomTypeId">The identifier of the room type.</param>
    /// <param name="ratePlanId">The identifier of the rate plan.</param>
    /// <param name="checkInDate">The day the first night begins.</param>
    /// <param name="checkOutDate">The day the guest leaves.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The price of the stay, or null when the room type does not exist.</returns>
    Task<decimal?> QuoteStayAsync(int roomTypeId, int ratePlanId, DateOnly checkInDate, DateOnly checkOutDate,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Indicates whether a Blocked or Out of service status covers any night of a range. Needs cleaning does not
    ///     prevent booking.
    /// </summary>
    /// <param name="roomId">The identifier of the room.</param>
    /// <param name="firstNight">The first night of the range.</param>
    /// <param name="lastNight">The last night of the range.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True when an operational status prevents booking the room on at least one night.</returns>
    Task<bool> HasBookingBlockingStatusAsync(int roomId, DateOnly firstNight, DateOnly lastNight,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Records that a room needs cleaning on a day, such as the day a guest checks out.
    /// </summary>
    /// <remarks>
    ///     A day that already has an operational status keeps it, so Blocked or Out of service are never replaced.
    /// </remarks>
    /// <param name="roomId">The identifier of the room.</param>
    /// <param name="date">The day the room must be cleaned.</param>
    /// <param name="reason">Why the room needs cleaning.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True when a Needs cleaning status was recorded.</returns>
    Task<bool> MarkRoomForCleaningAsync(int roomId, DateOnly date, string reason,
        CancellationToken cancellationToken);
}
