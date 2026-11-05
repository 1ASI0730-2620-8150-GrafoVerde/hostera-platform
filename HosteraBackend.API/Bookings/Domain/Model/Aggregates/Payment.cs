using HosteraBackend.API.Bookings.Domain.Model.Commands;
using HosteraBackend.API.Bookings.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Bookings.Domain.Model.Aggregates;

/// <summary>
///     Represents an amount received for a booking through a channel outside Hostera.
/// </summary>
/// <remarks>
///     Payments are immutable records: a mistake is corrected with another record, never by editing or deleting one.
///     Whether the booking accepts the payment and whether it fits the balance due are checked by the application
///     layer, because they depend on the other payments of the booking.
/// </remarks>
public partial class Payment
{
    /// <summary>
    ///     The longest reference accepted.
    /// </summary>
    public const int MaxReferenceLength = 80;

    /// <summary>
    ///     The longest idempotency key accepted.
    /// </summary>
    public const int MaxIdempotencyKeyLength = 100;

    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected Payment()
    {
    }

    /// <summary>
    ///     Creates a payment from a record command.
    /// </summary>
    /// <remarks>
    ///     This constructor acts as the command handler for RecordPaymentCommand.
    /// </remarks>
    /// <param name="command">The RecordPaymentCommand command</param>
    /// <param name="propertyId">The identifier of the property of the booking.</param>
    /// <exception cref="ArgumentNullException">Thrown when the command is null.</exception>
    /// <exception cref="ArgumentException">Thrown when a value is missing, out of range or too long.</exception>
    public Payment(RecordPaymentCommand command, int propertyId)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(propertyId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.BookingId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.Amount);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.RecordedBy);
        if (!Enum.IsDefined(command.Method))
            throw new ArgumentException("The payment method is not valid.", nameof(command));
        if (decimal.Round(command.Amount, 2) != command.Amount)
            throw new ArgumentException("A payment amount has at most two decimals.", nameof(command));

        var reference = string.IsNullOrWhiteSpace(command.Reference) ? null : command.Reference.Trim();
        if (reference?.Length > MaxReferenceLength)
            throw new ArgumentException($"A reference has at most {MaxReferenceLength} characters.",
                nameof(command));

        var idempotencyKey = string.IsNullOrWhiteSpace(command.IdempotencyKey) ? null : command.IdempotencyKey.Trim();
        if (idempotencyKey?.Length > MaxIdempotencyKeyLength)
            throw new ArgumentException($"An idempotency key has at most {MaxIdempotencyKeyLength} characters.",
                nameof(command));

        PropertyId = propertyId;
        BookingId = command.BookingId;
        Amount = command.Amount;
        Method = command.Method;
        PaidAt = command.PaidAt;
        Reference = reference;
        RecordedBy = command.RecordedBy;
        IdempotencyKey = idempotencyKey;
    }

    /// <summary>
    ///     Gets the server-generated identifier for this payment.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the identifier of the property of the booking.
    /// </summary>
    public int PropertyId { get; private set; }

    /// <summary>
    ///     Gets the identifier of the booking the payment is for.
    /// </summary>
    public int BookingId { get; private set; }

    /// <summary>
    ///     Gets the amount received.
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    ///     Gets the channel through which the amount was received.
    /// </summary>
    public EPaymentMethod Method { get; private set; }

    /// <summary>
    ///     Gets when the amount was received.
    /// </summary>
    public DateTimeOffset PaidAt { get; private set; }

    /// <summary>
    ///     Gets the reference of the payment, or null.
    /// </summary>
    public string? Reference { get; private set; }

    /// <summary>
    ///     Gets the identifier of the operator who recorded the payment.
    /// </summary>
    public int RecordedBy { get; private set; }

    /// <summary>
    ///     Gets the key the client sent to make the request idempotent, or null.
    /// </summary>
    public string? IdempotencyKey { get; private set; }

    /// <summary>
    ///     Indicates whether a repeated request asks for the same payment as this one.
    /// </summary>
    /// <param name="command">The repeated request.</param>
    /// <returns>True when the amount, method and booking match.</returns>
    public bool Matches(RecordPaymentCommand command) =>
        command.BookingId == BookingId && command.Amount == Amount && command.Method == Method;
}
