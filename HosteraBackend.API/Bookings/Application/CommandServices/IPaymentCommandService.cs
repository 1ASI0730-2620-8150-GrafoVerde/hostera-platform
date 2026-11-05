using HosteraBackend.API.Bookings.Application.Errors;
using HosteraBackend.API.Bookings.Domain.Model.Commands;
using HosteraBackend.API.Shared.Application.Patterns;

namespace HosteraBackend.API.Bookings.Application.CommandServices;

/// <summary>
///     Represents the payment command service of the Bookings bounded context.
/// </summary>
public interface IPaymentCommandService
{
    /// <summary>
    ///     Handles the record payment command.
    /// </summary>
    /// <param name="command">The <see cref="RecordPaymentCommand" /> command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    ///     The <see cref="RecordedPayment" /> with the new balance of the booking, or the <see cref="BookingError" />
    ///     that prevented it.
    /// </returns>
    Task<Result<RecordedPayment, BookingError>> Handle(RecordPaymentCommand command,
        CancellationToken cancellationToken);
}
