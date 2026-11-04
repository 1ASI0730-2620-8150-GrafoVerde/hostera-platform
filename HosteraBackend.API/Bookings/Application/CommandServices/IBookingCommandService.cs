using HosteraBackend.API.Bookings.Application.Errors;
using HosteraBackend.API.Bookings.Domain.Model.Aggregates;
using HosteraBackend.API.Bookings.Domain.Model.Commands;
using HosteraBackend.API.Shared.Application.Patterns;

namespace HosteraBackend.API.Bookings.Application.CommandServices;

/// <summary>
///     Represents the booking command service of the Bookings bounded context.
/// </summary>
public interface IBookingCommandService
{
    /// <summary>
    ///     Handles the create booking command.
    /// </summary>
    /// <param name="command">The <see cref="CreateBookingCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created <see cref="Booking" />, or the <see cref="BookingError" /> that prevented it.</returns>
    Task<Result<Booking, BookingError>> Handle(CreateBookingCommand command, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the update booking command.
    /// </summary>
    /// <param name="command">The <see cref="UpdateBookingCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated <see cref="Booking" />, or the <see cref="BookingError" /> that prevented it.</returns>
    Task<Result<Booking, BookingError>> Handle(UpdateBookingCommand command, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the transition booking command.
    /// </summary>
    /// <param name="command">The <see cref="TransitionBookingCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The booking in its new status, or the <see cref="BookingError" /> that prevented the change.</returns>
    Task<Result<Booking, BookingError>> Handle(TransitionBookingCommand command, CancellationToken cancellationToken);
}
