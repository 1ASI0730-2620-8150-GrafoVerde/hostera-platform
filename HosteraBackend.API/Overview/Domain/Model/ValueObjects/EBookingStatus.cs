namespace HosteraBackend.API.Overview.Domain.Model.ValueObjects;

/// <summary>
///     Status of a booking, as the Overview bounded context reads it from the Bookings bounded context.
/// </summary>
public enum EBookingStatus
{
    /// <summary>
    ///     The booking waits for its confirmation.
    /// </summary>
    Pending,

    /// <summary>
    ///     The booking was confirmed.
    /// </summary>
    Confirmed,

    /// <summary>
    ///     The guest is staying at the property.
    /// </summary>
    CheckedIn,

    /// <summary>
    ///     The guest left the property.
    /// </summary>
    CheckedOut,

    /// <summary>
    ///     The booking was cancelled.
    /// </summary>
    Cancelled,

    /// <summary>
    ///     The guest did not arrive.
    /// </summary>
    NoShow
}
