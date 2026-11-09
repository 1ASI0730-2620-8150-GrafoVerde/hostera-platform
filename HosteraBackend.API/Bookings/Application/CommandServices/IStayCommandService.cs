using HosteraBackend.API.Bookings.Application.Errors;
using HosteraBackend.API.Bookings.Domain.Model.Commands;
using HosteraBackend.API.Shared.Application.Patterns;

namespace HosteraBackend.API.Bookings.Application.CommandServices;

/// <summary>
///     Represents the stay command service of the Bookings bounded context, which starts and ends guest stays.
/// </summary>
public interface IStayCommandService
{
    /// <summary>
    ///     Handles the check-in booking command.
    /// </summary>
    /// <param name="command">The <see cref="CheckInBookingCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="CheckedInStay" />, or the <see cref="BookingError" /> that prevented it.</returns>
    Task<Result<CheckedInStay, BookingError>> Handle(CheckInBookingCommand command,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the check-out booking command.
    /// </summary>
    /// <param name="command">The <see cref="CheckOutBookingCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="CheckedOutStay" />, or the <see cref="BookingError" /> that prevented it.</returns>
    Task<Result<CheckedOutStay, BookingError>> Handle(CheckOutBookingCommand command,
        CancellationToken cancellationToken);
}
