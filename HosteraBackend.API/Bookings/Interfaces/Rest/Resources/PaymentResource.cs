namespace HosteraBackend.API.Bookings.Interfaces.Rest.Resources;

/// <summary>
///     Payment resource for REST API
/// </summary>
/// <param name="Id">The unique identifier of the payment</param>
/// <param name="PropertyId">The identifier of the property of the booking</param>
/// <param name="BookingId">The identifier of the booking the payment is for</param>
/// <param name="Amount">The amount received</param>
/// <param name="Method">The channel: cash, card-terminal, bank-transfer or other</param>
/// <param name="PaidAt">When the amount was received</param>
/// <param name="Reference">The reference of the payment, or null</param>
/// <param name="RecordedBy">The operator who recorded the payment</param>
public record PaymentResource(
    int Id,
    int PropertyId,
    int BookingId,
    decimal Amount,
    string Method,
    DateTimeOffset PaidAt,
    string? Reference,
    int RecordedBy);
