namespace HosteraBackend.API.Overview.Domain.Model.ValueObjects;

/// <summary>
///     Status of a room on a day, combining the bookings assigned to it and its status periods.
/// </summary>
public enum ERoomDayStatus
{
    /// <summary>
    ///     The room can be sold.
    /// </summary>
    Available,

    /// <summary>
    ///     A pending or confirmed booking holds the room.
    /// </summary>
    Booked,

    /// <summary>
    ///     A checked-in guest stays in the room.
    /// </summary>
    Occupied,

    /// <summary>
    ///     The room must be cleaned before the next guest.
    /// </summary>
    NeedsCleaning,

    /// <summary>
    ///     The room cannot be sold.
    /// </summary>
    Blocked,

    /// <summary>
    ///     The room is under repair.
    /// </summary>
    OutOfService
}
