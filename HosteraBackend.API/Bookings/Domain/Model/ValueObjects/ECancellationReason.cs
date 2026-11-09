namespace HosteraBackend.API.Bookings.Domain.Model.ValueObjects;

/// <summary>
///     Why a booking ended without a stay.
/// </summary>
public enum ECancellationReason
{
    /// <summary>
    ///     The guest asked to cancel.
    /// </summary>
    GuestRequest,

    /// <summary>
    ///     The guest's plans changed.
    /// </summary>
    ChangeOfPlans,

    /// <summary>
    ///     The booking repeats another one.
    /// </summary>
    DuplicateBooking,

    /// <summary>
    ///     The property cannot host the stay.
    /// </summary>
    PropertyUnable,

    /// <summary>
    ///     Another reason, explained in the cancellation note.
    /// </summary>
    Other
}
