namespace HosteraBackend.API.Bookings.Domain.Model.ValueObjects;

/// <summary>
///     The channel outside Hostera through which a payment was received.
/// </summary>
public enum EPaymentMethod
{
    /// <summary>
    ///     Cash received at the property.
    /// </summary>
    Cash,

    /// <summary>
    ///     A card charged on a payment terminal.
    /// </summary>
    CardTerminal,

    /// <summary>
    ///     A bank transfer.
    /// </summary>
    BankTransfer,

    /// <summary>
    ///     Another channel, described in the reference.
    /// </summary>
    Other
}
