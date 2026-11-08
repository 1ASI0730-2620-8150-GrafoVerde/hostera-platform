using HosteraBackend.API.Bookings.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Bookings.Domain.Model.Commands;

/// <summary>
///     Command to record a payment received for a booking
/// </summary>
/// <param name="BookingId">The identifier of the booking</param>
/// <param name="Amount">The amount received, up to the balance due of the booking</param>
/// <param name="Method">The EPaymentMethod through which the amount was received</param>
/// <param name="PaidAt">When the amount was received</param>
/// <param name="Reference">An optional reference, such as the terminal voucher or transfer number</param>
/// <param name="RecordedBy">The identifier of the operator who records the payment</param>
/// <param name="IdempotencyKey">
///     An optional key chosen by the client, so that repeating the same request does not record the payment twice
/// </param>
/// <param name="ExpectedVersion">
///     The version of the booking the client read, or null to skip the check; a newer version rejects the change
/// </param>
public record RecordPaymentCommand(
    int BookingId,
    decimal Amount,
    EPaymentMethod Method,
    DateTimeOffset PaidAt,
    string? Reference,
    int RecordedBy,
    string? IdempotencyKey,
    int? ExpectedVersion = null);
