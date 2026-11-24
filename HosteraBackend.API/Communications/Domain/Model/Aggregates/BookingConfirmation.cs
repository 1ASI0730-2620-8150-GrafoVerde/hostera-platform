using HosteraBackend.API.Communications.Domain.Model.Commands;
using HosteraBackend.API.Communications.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Communications.Domain.Model.Aggregates;

/// <summary>
///     Represents the email that confirms a new booking to its guest, and what happened when it was sent.
/// </summary>
/// <remarks>
///     A booking confirmation starts as Pending and ends once as Sent, Failed or Skipped. It keeps the recipient and
///     the booking reference so that staff can tell whether a guest was emailed, even when the booking changes later.
/// </remarks>
public partial class BookingConfirmation
{
    /// <summary>
    ///     The longest reason kept for a failed or skipped confirmation.
    /// </summary>
    public const int MaxReasonLength = 500;

    /// <summary>
    ///     Protected parameterless constructor for EF Core.
    /// </summary>
    protected BookingConfirmation()
    {
        BookingCode = null!;
        RecipientEmail = null!;
    }

    /// <summary>
    ///     Creates a pending booking confirmation from a send command.
    /// </summary>
    /// <remarks>
    ///     This constructor acts as the command handler for SendBookingConfirmationCommand.
    /// </remarks>
    /// <param name="command">The SendBookingConfirmationCommand command</param>
    /// <exception cref="ArgumentNullException">Thrown when the command is null.</exception>
    /// <exception cref="ArgumentException">Thrown when an identifier, the booking code or the recipient is missing.</exception>
    public BookingConfirmation(SendBookingConfirmationCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.PropertyId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(command.BookingId);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.BookingCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(command.GuestEmail);

        PropertyId = command.PropertyId;
        BookingId = command.BookingId;
        BookingCode = command.BookingCode.Trim();
        RecipientEmail = command.GuestEmail.Trim();
        Language = command.Language;
        Status = EDeliveryStatus.Pending;
    }

    /// <summary>
    ///     Gets the server-generated identifier for this booking confirmation.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Gets the identifier of the property of the booking.
    /// </summary>
    public int PropertyId { get; private set; }

    /// <summary>
    ///     Gets the identifier of the confirmed booking.
    /// </summary>
    public int BookingId { get; private set; }

    /// <summary>
    ///     Gets the code of the confirmed booking, such as BKG-1048.
    /// </summary>
    public string BookingCode { get; private set; }

    /// <summary>
    ///     Gets the email address of the guest who receives the confirmation.
    /// </summary>
    public string RecipientEmail { get; private set; }

    /// <summary>
    ///     Gets the language in which the confirmation is written.
    /// </summary>
    public EEmailLanguage Language { get; private set; }

    /// <summary>
    ///     Gets where the confirmation stands.
    /// </summary>
    public EDeliveryStatus Status { get; private set; }

    /// <summary>
    ///     Gets the identifier the email provider gave to the sent email, or null when it was not sent.
    /// </summary>
    public string? ProviderMessageId { get; private set; }

    /// <summary>
    ///     Gets why the confirmation failed or was skipped, or null.
    /// </summary>
    public string? Reason { get; private set; }

    /// <summary>
    ///     Gets when the confirmation was sent, failed or skipped, or null while it is pending.
    /// </summary>
    public DateTimeOffset? ProcessedAt { get; private set; }

    /// <summary>
    ///     Records that the email provider accepted the confirmation.
    /// </summary>
    /// <param name="providerMessageId">The identifier the email provider gave to the email.</param>
    /// <param name="sentAt">The moment the email was accepted.</param>
    /// <exception cref="InvalidOperationException">Thrown when the confirmation is no longer pending.</exception>
    public void MarkSent(string? providerMessageId, DateTimeOffset sentAt)
    {
        Complete(EDeliveryStatus.Sent, sentAt);
        ProviderMessageId = string.IsNullOrWhiteSpace(providerMessageId) ? null : providerMessageId.Trim();
    }

    /// <summary>
    ///     Records that the email provider rejected the confirmation or could not be reached.
    /// </summary>
    /// <param name="reason">Why the email was not sent.</param>
    /// <param name="failedAt">The moment the email failed.</param>
    /// <exception cref="InvalidOperationException">Thrown when the confirmation is no longer pending.</exception>
    public void MarkFailed(string? reason, DateTimeOffset failedAt)
    {
        Complete(EDeliveryStatus.Failed, failedAt);
        Reason = Shorten(reason);
    }

    /// <summary>
    ///     Records that the confirmation was not sent because sending is not configured.
    /// </summary>
    /// <param name="reason">Why the email was not sent.</param>
    /// <param name="skippedAt">The moment the email was skipped.</param>
    /// <exception cref="InvalidOperationException">Thrown when the confirmation is no longer pending.</exception>
    public void MarkSkipped(string? reason, DateTimeOffset skippedAt)
    {
        Complete(EDeliveryStatus.Skipped, skippedAt);
        Reason = Shorten(reason);
    }

    private void Complete(EDeliveryStatus status, DateTimeOffset processedAt)
    {
        if (Status != EDeliveryStatus.Pending)
            throw new InvalidOperationException($"A {Status} booking confirmation cannot change.");

        Status = status;
        ProcessedAt = processedAt;
    }

    private static string? Shorten(string? reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return null;

        var trimmed = reason.Trim();
        return trimmed.Length <= MaxReasonLength ? trimmed : trimmed[..MaxReasonLength];
    }
}
