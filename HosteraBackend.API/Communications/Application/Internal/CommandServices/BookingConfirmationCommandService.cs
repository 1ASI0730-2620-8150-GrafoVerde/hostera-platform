using HosteraBackend.API.Communications.Application.CommandServices;
using HosteraBackend.API.Communications.Application.Errors;
using HosteraBackend.API.Communications.Application.Internal.EmailTemplates;
using HosteraBackend.API.Communications.Application.Internal.OutboundServices;
using HosteraBackend.API.Communications.Domain.Model.Aggregates;
using HosteraBackend.API.Communications.Domain.Model.Commands;
using HosteraBackend.API.Communications.Domain.Model.ValueObjects;
using HosteraBackend.API.Communications.Domain.Repositories;
using HosteraBackend.API.Shared.Application.Internal.OutboundServices;
using HosteraBackend.API.Shared.Application.Patterns;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.Communications.Application.Internal.CommandServices;

/// <summary>
///     Represents the booking confirmation command service of the Communications bounded context.
/// </summary>
/// <param name="bookingConfirmationRepository">
///     The <see cref="IBookingConfirmationRepository" /> to use.
/// </param>
/// <param name="bookingConfirmationEmailTemplate">
///     The <see cref="BookingConfirmationEmailTemplate" /> that writes the email.
/// </param>
/// <param name="emailService">
///     The <see cref="IEmailService" /> that hands the email to the email provider.
/// </param>
/// <param name="clock">
///     The <see cref="IClock" /> that provides the current moment.
/// </param>
/// <param name="unitOfWork">
///     The <see cref="IUnitOfWork" /> to use.
/// </param>
/// <param name="logger">
///     The logger that reports the outcome of each confirmation.
/// </param>
public class BookingConfirmationCommandService(
    IBookingConfirmationRepository bookingConfirmationRepository,
    BookingConfirmationEmailTemplate bookingConfirmationEmailTemplate,
    IEmailService emailService,
    IClock clock,
    IUnitOfWork unitOfWork,
    ILogger<BookingConfirmationCommandService> logger)
    : IBookingConfirmationCommandService
{
    /// <inheritdoc />
    public async Task<Result<BookingConfirmation, CommunicationError>> Handle(SendBookingConfirmationCommand command,
        CancellationToken cancellationToken)
    {
        BookingConfirmation bookingConfirmation;
        try
        {
            bookingConfirmation = new BookingConfirmation(command);
        }
        catch (ArgumentException)
        {
            return new Result<BookingConfirmation, CommunicationError>.Failure(CommunicationError.InvalidInput);
        }

        var email = bookingConfirmationEmailTemplate.Compose(command);
        var delivery = await emailService.SendAsync(email, cancellationToken);

        switch (delivery.Status)
        {
            case EDeliveryStatus.Sent:
                bookingConfirmation.MarkSent(delivery.ProviderMessageId, clock.Now);
                logger.LogInformation("Booking confirmation of {BookingCode} sent to {Recipient} with message {MessageId}",
                    bookingConfirmation.BookingCode, bookingConfirmation.RecipientEmail, delivery.ProviderMessageId);
                break;
            case EDeliveryStatus.Skipped:
                bookingConfirmation.MarkSkipped(delivery.Reason, clock.Now);
                logger.LogInformation("Booking confirmation of {BookingCode} skipped: {Reason}",
                    bookingConfirmation.BookingCode, delivery.Reason);
                break;
            default:
                bookingConfirmation.MarkFailed(delivery.Reason, clock.Now);
                logger.LogWarning("Booking confirmation of {BookingCode} to {Recipient} failed: {Reason}",
                    bookingConfirmation.BookingCode, bookingConfirmation.RecipientEmail, delivery.Reason);
                break;
        }

        await bookingConfirmationRepository.AddAsync(bookingConfirmation, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<BookingConfirmation, CommunicationError>.Success(bookingConfirmation);
    }
}
