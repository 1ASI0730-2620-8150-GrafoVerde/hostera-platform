namespace HosteraBackend.API.Rooms.Domain.Model.ValueObjects;

/// <summary>
///     Room condition set by property staff that affects whether the room can be sold.
/// </summary>
public enum EOperationalStatus
{
    /// <summary>
    ///     The room is usable but withheld from sale.
    /// </summary>
    Blocked,

    /// <summary>
    ///     The room cannot be used because of maintenance or damage.
    /// </summary>
    OutOfService,

    /// <summary>
    ///     The room must be prepared before the next guest. It can still be booked.
    /// </summary>
    NeedsCleaning
}
