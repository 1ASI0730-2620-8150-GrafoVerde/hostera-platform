namespace HosteraBackend.API.Bookings.Domain.Model.ValueObjects;

/// <summary>
///     How much of a booking total has been paid.
/// </summary>
public enum EPaymentStatus
{
    /// <summary>
    ///     No payment has been recorded and something is owed.
    /// </summary>
    Unpaid,

    /// <summary>
    ///     Some payments were recorded, but part of the total is still owed.
    /// </summary>
    PartiallyPaid,

    /// <summary>
    ///     Nothing is owed.
    /// </summary>
    Paid
}
