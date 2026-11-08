using HosteraBackend.API.Bookings.Application.CommandServices;
using HosteraBackend.API.Bookings.Application.Errors;
using HosteraBackend.API.Bookings.Application.Internal.OutboundServices.ACL;
using HosteraBackend.API.Bookings.Domain.Model.Commands;
using HosteraBackend.API.Bookings.Domain.Repositories;
using HosteraBackend.API.Shared.Application.Internal.OutboundServices;
using HosteraBackend.API.Shared.Application.Patterns;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.Bookings.Application.Internal.CommandServices;

/// <summary>
///     Represents the stay command service of the Bookings bounded context.
/// </summary>
/// <remarks>
///     A check-in and the key cards of its guest are saved together: when Access Control cannot issue the key cards,
///     the booking stays Confirmed. A check-out and the end of its key cards are saved together too; marking the room
///     for cleaning follows as a separate change.
/// </remarks>
/// <param name="bookingRepository">
///     The <see cref="IBookingRepository" /> to use.
/// </param>
/// <param name="paymentRepository">
///     The <see cref="IPaymentRepository" /> used to know what the booking still owes.
/// </param>
/// <param name="externalGuestKeyCardService">
///     The <see cref="ExternalGuestKeyCardService" /> that issues and ends key cards.
/// </param>
/// <param name="externalRoomService">
///     The <see cref="ExternalRoomService" /> that marks rooms for cleaning.
/// </param>
/// <param name="clock">
///     The <see cref="IClock" /> that provides the current moment and day of the property.
/// </param>
public class StayCommandService(
    IBookingRepository bookingRepository,
    IPaymentRepository paymentRepository,
    ExternalGuestKeyCardService externalGuestKeyCardService,
    ExternalRoomService externalRoomService,
    IClock clock)
    : IStayCommandService
{
    /// <summary>
    ///     The time of the check-out day when the key cards of a guest stop opening the room.
    /// </summary>
    public static readonly TimeOnly CheckOutTime = new(11, 0);

    /// <inheritdoc />
    public async Task<Result<CheckedInStay, BookingError>> Handle(CheckInBookingCommand command,
        CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.FindByIdAsync(command.BookingId, cancellationToken);
        if (booking is null)
            return new Result<CheckedInStay, BookingError>.Failure(BookingError.BookingNotFound);
        if (command.ExpectedVersion is { } expectedVersion && expectedVersion != booking.Version)
            return new Result<CheckedInStay, BookingError>.Failure(BookingError.ConcurrencyConflict);
        if (command.OperatorId <= 0)
            return new Result<CheckedInStay, BookingError>.Failure(BookingError.InvalidInput);

        var today = clock.Today;
        if (!booking.CanBeCheckedIn(today))
            return new Result<CheckedInStay, BookingError>.Failure(BookingError.InvalidStatusChange);
        if (!command.DocumentVerified || string.IsNullOrWhiteSpace(command.DocumentNumber))
            return new Result<CheckedInStay, BookingError>.Failure(BookingError.IdentityNotVerified);
        if (command.KeyCardIds.Count == 0)
            return new Result<CheckedInStay, BookingError>.Failure(BookingError.KeyCardRequired);

        var now = clock.Now;
        try
        {
            booking.CheckIn(today, command.DocumentType, command.DocumentNumber, command.DocumentVerified, now,
                command.OperatorId);
        }
        catch (ArgumentException)
        {
            return new Result<CheckedInStay, BookingError>.Failure(BookingError.InvalidInput);
        }

        // Access Control saves the key cards together with the check-in, or nothing when it rejects them
        IReadOnlyList<int> keyCardIds;
        string? errorCode;
        try
        {
            (keyCardIds, errorCode) = await externalGuestKeyCardService.IssueGuestKeyCardsAsync(booking, now,
                clock.AtLocalTime(booking.CheckOutDate, CheckOutTime), command.KeyCardIds, command.OperatorId,
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return new Result<CheckedInStay, BookingError>.Failure(BookingError.ConcurrencyConflict);
        }
        if (errorCode is not null)
            return new Result<CheckedInStay, BookingError>.Failure(BookingError.KeyCardsNotIssued);

        return new Result<CheckedInStay, BookingError>.Success(new CheckedInStay(booking, keyCardIds));
    }

    /// <inheritdoc />
    public async Task<Result<CheckedOutStay, BookingError>> Handle(CheckOutBookingCommand command,
        CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.FindByIdAsync(command.BookingId, cancellationToken);
        if (booking is null)
            return new Result<CheckedOutStay, BookingError>.Failure(BookingError.BookingNotFound);
        if (command.ExpectedVersion is { } expectedVersion && expectedVersion != booking.Version)
            return new Result<CheckedOutStay, BookingError>.Failure(BookingError.ConcurrencyConflict);
        if (command.OperatorId <= 0)
            return new Result<CheckedOutStay, BookingError>.Failure(BookingError.InvalidInput);
        if (!booking.CanBeCheckedOut)
            return new Result<CheckedOutStay, BookingError>.Failure(BookingError.InvalidStatusChange);

        var paidAmounts = await paymentRepository.SumAmountsByBookingIdsAsync([booking.Id], cancellationToken);
        var balanceDue = booking.BalanceDue(paidAmounts.GetValueOrDefault(booking.Id));
        if (balanceDue > 0)
            return new Result<CheckedOutStay, BookingError>.Failure(BookingError.BalanceDue);

        var now = clock.Now;
        try
        {
            booking.CheckOut(balanceDue, command.RoomCondition, command.DepartureNote, now, command.OperatorId);
        }
        catch (ArgumentException)
        {
            return new Result<CheckedOutStay, BookingError>.Failure(BookingError.InvalidInput);
        }

        // Access Control saves the end of the key cards together with the check-out
        int endedKeyCards;
        try
        {
            endedKeyCards = await externalGuestKeyCardService.EndGuestKeyCardsAsync(booking.Id, now,
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return new Result<CheckedOutStay, BookingError>.Failure(BookingError.ConcurrencyConflict);
        }

        // The room needs cleaning on the departure day, unless the next guest already holds it that night
        var today = clock.Today;
        var roomMarkedForCleaning =
            !await bookingRepository.ExistsRoomHoldAsync(booking.RoomId, today, today.AddDays(1), booking.Id,
                cancellationToken)
            && await externalRoomService.MarkRoomForCleaningAsync(booking.RoomId, today, booking.Code,
                cancellationToken);

        return new Result<CheckedOutStay, BookingError>.Success(
            new CheckedOutStay(booking, endedKeyCards, roomMarkedForCleaning));
    }
}
