namespace HosteraBackend.API.Bookings.Domain.Model.ValueObjects;

/// <summary>
///     Where a booking stands in its lifecycle.
/// </summary>
public enum EBookingStatus
{
    /// <summary>
    ///     Created and awaiting confirmation. It holds its room.
    /// </summary>
    Pending,

    /// <summary>
    ///     Approved. It holds its room and can still be edited or cancelled.
    /// </summary>
    Confirmed,

    /// <summary>
    ///     The guest is staying. It holds its room.
    /// </summary>
    CheckedIn,

    /// <summary>
    ///     The stay ended.
    /// </summary>
    CheckedOut,

    /// <summary>
    ///     Ended without a stay.
    /// </summary>
    Cancelled,

    /// <summary>
    ///     The guest did not arrive.
    /// </summary>
    NoShow
}
