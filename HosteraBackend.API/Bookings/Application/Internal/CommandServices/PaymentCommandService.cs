using HosteraBackend.API.Bookings.Application.CommandServices;
using HosteraBackend.API.Bookings.Application.Errors;
using HosteraBackend.API.Bookings.Domain.Model.Aggregates;
using HosteraBackend.API.Bookings.Domain.Model.Commands;
using HosteraBackend.API.Bookings.Domain.Repositories;
using HosteraBackend.API.Shared.Application.Internal.OutboundServices;
using HosteraBackend.API.Shared.Application.Patterns;
using HosteraBackend.API.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.Bookings.Application.Internal.CommandServices;

/// <summary>
///     Represents the payment command service of the Bookings bounded context.
/// </summary>
/// <param name="paymentRepository">
///     The <see cref="IPaymentRepository" /> to use.
/// </param>
/// <param name="bookingRepository">
///     The <see cref="IBookingRepository" /> to use.
/// </param>
/// <param name="clock">
///     The <see cref="IClock" /> that provides the current moment.
/// </param>
/// <param name="unitOfWork">
///     The <see cref="IUnitOfWork" /> to use.
/// </param>
public class PaymentCommandService(
    IPaymentRepository paymentRepository,
    IBookingRepository bookingRepository,
    IClock clock,
    IUnitOfWork unitOfWork)
    : IPaymentCommandService
{
    /// <summary>
    ///     How far ahead of the server clock a payment moment may be, to absorb small clock differences.
    /// </summary>
    private static readonly TimeSpan ClockTolerance = TimeSpan.FromMinutes(5);

    /// <inheritdoc />
    public async Task<Result<RecordedPayment, BookingError>> Handle(RecordPaymentCommand command,
        CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.FindByIdAsync(command.BookingId, cancellationToken);
        if (booking is null)
            return Failure(BookingError.BookingNotFound);

        // A repeated request returns the payment it recorded the first time, whatever the booking looks like now
        var idempotencyKey = command.IdempotencyKey?.Trim();
        if (!string.IsNullOrEmpty(idempotencyKey))
        {
            var replay = await ReplayAsync(booking, command, idempotencyKey, cancellationToken);
            if (replay is not null)
                return replay;
        }

        if (command.ExpectedVersion is { } expectedVersion && expectedVersion != booking.Version)
            return Failure(BookingError.ConcurrencyConflict);
        if (!booking.AcceptsPayments)
            return Failure(BookingError.PaymentsNotAccepted);
        if (command.Amount <= 0 || decimal.Round(command.Amount, 2) != command.Amount)
            return Failure(BookingError.InvalidPaymentAmount);
        if (command.PaidAt > clock.Now + ClockTolerance)
            return Failure(BookingError.InvalidPaymentDate);

        var paidAmount = await PaidAmountAsync(booking.Id, cancellationToken);
        if (command.Amount > booking.BalanceDue(paidAmount))
            return Failure(BookingError.PaymentExceedsBalance);

        Payment payment;
        try
        {
            payment = new Payment(command, booking.PropertyId);
        }
        catch (ArgumentException)
        {
            return Failure(BookingError.InvalidInput);
        }

        // The new version of the booking makes a concurrent payment fail instead of exceeding the balance
        booking.AcceptPayment();
        await paymentRepository.AddAsync(payment, cancellationToken);
        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Failure(BookingError.ConcurrencyConflict);
        }
        catch (DbUpdateException) when (!string.IsNullOrEmpty(idempotencyKey))
        {
            // Another request with the same key was recorded first
            paymentRepository.Remove(payment);
            return await ReplayAsync(booking, command, idempotencyKey, cancellationToken)
                   ?? Failure(BookingError.IdempotencyKeyReused);
        }

        var newPaidAmount = paidAmount + payment.Amount;
        return new Result<RecordedPayment, BookingError>.Success(new RecordedPayment(payment,
            booking.BalanceDue(newPaidAmount), booking.PaymentStatus(newPaidAmount), booking.Version, false));
    }

    private async Task<Result<RecordedPayment, BookingError>?> ReplayAsync(Booking booking,
        RecordPaymentCommand command, string idempotencyKey, CancellationToken cancellationToken)
    {
        var existing = await paymentRepository.FindByBookingIdAndIdempotencyKeyAsync(booking.Id, idempotencyKey,
            cancellationToken);
        if (existing is null)
            return null;
        if (!existing.Matches(command))
            return Failure(BookingError.IdempotencyKeyReused);

        var paidAmount = await PaidAmountAsync(booking.Id, cancellationToken);
        return new Result<RecordedPayment, BookingError>.Success(new RecordedPayment(existing,
            booking.BalanceDue(paidAmount), booking.PaymentStatus(paidAmount), booking.Version, true));
    }

    private async Task<decimal> PaidAmountAsync(int bookingId, CancellationToken cancellationToken)
    {
        var paidAmounts = await paymentRepository.SumAmountsByBookingIdsAsync([bookingId], cancellationToken);
        return paidAmounts.GetValueOrDefault(bookingId);
    }

    private static Result<RecordedPayment, BookingError> Failure(BookingError error) =>
        new Result<RecordedPayment, BookingError>.Failure(error);
}
