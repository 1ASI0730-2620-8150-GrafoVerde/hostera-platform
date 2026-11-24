using HosteraBackend.API.Communications.Application.Errors;
using HosteraBackend.API.Communications.Domain.Model.Aggregates;
using HosteraBackend.API.Communications.Domain.Model.Commands;
using HosteraBackend.API.Shared.Application.Patterns;

namespace HosteraBackend.API.Communications.Application.CommandServices;

/// <summary>
///     Represents the booking confirmation command service of the Communications bounded context.
/// </summary>
public interface IBookingConfirmationCommandService
{
    /// <summary>
    ///     Handles the send booking confirmation command.
    /// </summary>
    /// <remarks>
    ///     A confirmation that the email provider rejects, or that is skipped because sending is not configured, is
    ///     still a success: the returned <see cref="BookingConfirmation" /> records what happened.
    /// </remarks>
    /// <param name="command">The <see cref="SendBookingConfirmationCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     The recorded <see cref="BookingConfirmation" />, or the <see cref="CommunicationError" /> that prevented it.
    /// </returns>
    Task<Result<BookingConfirmation, CommunicationError>> Handle(SendBookingConfirmationCommand command,
        CancellationToken cancellationToken);
}
