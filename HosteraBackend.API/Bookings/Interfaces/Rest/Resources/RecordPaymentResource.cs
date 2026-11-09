using System.ComponentModel.DataAnnotations;

namespace HosteraBackend.API.Bookings.Interfaces.Rest.Resources;

/// <summary>
///     Record payment resource for REST API
/// </summary>
/// <param name="Amount">The amount received, up to the balance due of the booking</param>
/// <param name="Method">The channel: cash, card-terminal, bank-transfer or other</param>
/// <param name="PaidAt">When the amount was received</param>
/// <param name="Reference">An optional reference, such as the terminal voucher or transfer number</param>
/// <param name="RecordedBy">The identifier of the operator who records the payment</param>
public record RecordPaymentResource(
    decimal Amount,
    [Required] string Method,
    DateTimeOffset PaidAt,
    [MaxLength(80)] string? Reference,
    [Range(1, int.MaxValue)] int RecordedBy);
