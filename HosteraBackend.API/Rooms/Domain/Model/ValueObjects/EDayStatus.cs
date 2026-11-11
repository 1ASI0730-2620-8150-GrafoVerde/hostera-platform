namespace HosteraBackend.API.Rooms.Domain.Model.ValueObjects;

/// <summary>
///     Resulting status of a room on one day.
/// </summary>
/// <remarks>
///     A room assignment decides the day first (Booked or Occupied), then the covering status period, and the room is
///     Available otherwise.
/// </remarks>
public enum EDayStatus
{
    /// <summary>
    ///     Nothing prevents selling the room on the day.
    /// </summary>
    Available,

    /// <summary>
    ///     A booking that has not checked in holds the room on the day.
    /// </summary>
    Booked,

    /// <summary>
    ///     A checked-in guest stays in the room on the day.
    /// </summary>
    Occupied,

    /// <summary>
    ///     The room must be prepared before the next guest.
    /// </summary>
    NeedsCleaning,

    /// <summary>
    ///     The room is usable but withheld from sale.
    /// </summary>
    Blocked,

    /// <summary>
    ///     The room cannot be used because of maintenance or damage.
    /// </summary>
    OutOfService
}
