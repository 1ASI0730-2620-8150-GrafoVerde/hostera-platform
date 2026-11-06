using HosteraBackend.API.Bookings.Application.CommandServices;
using HosteraBackend.API.Bookings.Application.Errors;
using HosteraBackend.API.Bookings.Application.Internal.OutboundServices.ACL;
using HosteraBackend.API.Bookings.Domain.Model.Aggregates;
using HosteraBackend.API.Bookings.Domain.Model.Commands;
using HosteraBackend.API.Bookings.Domain.Model.ValueObjects;
using HosteraBackend.API.Bookings.Domain.Repositories;
using HosteraBackend.API.Shared.Application.Internal.OutboundServices;
using HosteraBackend.API.Shared.Application.Patterns;
using HosteraBackend.API.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.Bookings.Application.Internal.CommandServices;

/// <summary>
///     Represents the booking command service of the Bookings bounded context.
/// </summary>
/// <param name="bookingRepository">
///     The <see cref="IBookingRepository" /> to use.
/// </param>
/// <param name="externalRoomService">
///     The <see cref="ExternalRoomService" /> that reads the Rooms bounded context.
/// </param>
/// <param name="clock">
///     The <see cref="IClock" /> that provides the current moment and day of the property.
/// </param>
/// <param name="unitOfWork">
///     The <see cref="IUnitOfWork" /> to use.
/// </param>
public class BookingCommandService(
    IBookingRepository bookingRepository,
    ExternalRoomService externalRoomService,
    IClock clock,
    IUnitOfWork unitOfWork)
    : IBookingCommandService
{
    private const int MaxCodeAttempts = 10;

    /// <inheritdoc />
    public async Task<Result<Booking, BookingError>> Handle(CreateBookingCommand command,
        CancellationToken cancellationToken)
    {
        if (!AreValidStayDates(command.CheckInDate, command.CheckOutDate))
            return Failure(BookingError.InvalidStayDates);

        var bookabilityError = await ValidateBookabilityAsync(command.PropertyId, command.RoomTypeId, command.RoomId,
            command.RatePlanId, command.Guests, command.CheckInDate, command.CheckOutDate, null, cancellationToken);
        if (bookabilityError is { } error)
            return Failure(error);

        var totalAmount = await externalRoomService.QuoteStayAsync(command.RoomTypeId, command.RatePlanId,
            command.CheckInDate, command.CheckOutDate, cancellationToken);
        if (totalAmount is null)
            return Failure(BookingError.RoomTypeNotFound);

        // The unique code index rejects a code that another request took first. A retry never proposes a code that
        // already collided, even when the other request has not finished saving and the latest number looks the same
        var takenNumber = 0;
        for (var attempt = 1;; attempt++)
        {
            var latestNumber = Math.Max(takenNumber,
                await bookingRepository.FindLatestCodeNumberByPropertyIdAsync(command.PropertyId, cancellationToken));
            var code = Booking.NextCode(command.PropertyId, latestNumber);
            Booking booking;
            try
            {
                booking = new Booking(command, code, totalAmount.Value);
            }
            catch (ArgumentException)
            {
                return Failure(BookingError.InvalidInput);
            }

            await bookingRepository.AddAsync(booking, cancellationToken);
            try
            {
                await unitOfWork.CompleteAsync(cancellationToken);
                return new Result<Booking, BookingError>.Success(booking);
            }
            catch (DbUpdateException) when (attempt < MaxCodeAttempts)
            {
                bookingRepository.Remove(booking);
                takenNumber = Booking.NumberOf(code);
            }
        }
    }

    /// <inheritdoc />
    public async Task<Result<Booking, BookingError>> Handle(UpdateBookingCommand command,
        CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.FindByIdAsync(command.BookingId, cancellationToken);
        if (booking is null)
            return Failure(BookingError.BookingNotFound);

        if (!booking.IsEditable)
            return Failure(BookingError.NotEditable);

        if (!AreValidStayDates(command.CheckInDate, command.CheckOutDate))
            return Failure(BookingError.InvalidStayDates);

        var bookabilityError = await ValidateBookabilityAsync(booking.PropertyId, command.RoomTypeId,
            command.RoomId, command.RatePlanId, command.Guests, command.CheckInDate, command.CheckOutDate, booking,
            cancellationToken);
        if (bookabilityError is { } error)
            return Failure(error);

        var totalAmount = booking.TotalAmount;
        if (!booking.HasSamePricingAs(command.RoomTypeId, command.RatePlanId, command.CheckInDate,
                command.CheckOutDate))
        {
            var quote = await externalRoomService.QuoteStayAsync(command.RoomTypeId, command.RatePlanId,
                command.CheckInDate, command.CheckOutDate, cancellationToken);
            if (quote is null)
                return Failure(BookingError.RoomTypeNotFound);
            totalAmount = quote.Value;
        }

        try
        {
            booking.Update(command, totalAmount);
        }
        catch (ArgumentException)
        {
            return Failure(BookingError.InvalidInput);
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Booking, BookingError>.Success(booking);
    }

    /// <inheritdoc />
    public async Task<Result<Booking, BookingError>> Handle(TransitionBookingCommand command,
        CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.FindByIdAsync(command.BookingId, cancellationToken);
        if (booking is null)
            return Failure(BookingError.BookingNotFound);

        if (command.OperatorId <= 0)
            return Failure(BookingError.InvalidInput);

        var today = clock.Today;
        if (!booking.AllowedTransitions(today).Contains(command.TargetStatus))
            return Failure(BookingError.InvalidStatusChange);

        switch (command.TargetStatus)
        {
            case EBookingStatus.Confirmed:
                booking.Confirm(clock.Now, command.OperatorId);
                break;
            case EBookingStatus.Cancelled:
                if (command.CancellationReason is not { } reason)
                    return Failure(BookingError.CancellationReasonRequired);
                if (reason == ECancellationReason.Other && string.IsNullOrWhiteSpace(command.CancellationNote))
                    return Failure(BookingError.CancellationNoteRequired);
                booking.Cancel(reason, command.CancellationNote, clock.Now, command.OperatorId);
                break;
            case EBookingStatus.NoShow:
                booking.MarkNoShow(today, clock.Now, command.OperatorId);
                break;
            case EBookingStatus.Pending:
                // A restored booking holds its room again, so nobody else may have taken it meanwhile
                if (await bookingRepository.ExistsRoomHoldAsync(booking.RoomId, booking.CheckInDate,
                        booking.CheckOutDate, booking.Id, cancellationToken)
                    || await externalRoomService.IsRoomOutOfOrderAsync(booking.RoomId, booking.CheckInDate,
                        booking.CheckOutDate, cancellationToken))
                    return Failure(BookingError.RoomUnavailable);
                booking.Restore(today);
                break;
            default:
                return Failure(BookingError.InvalidStatusChange);
        }

        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Booking, BookingError>.Success(booking);
    }

    private static bool AreValidStayDates(DateOnly checkInDate, DateOnly checkOutDate)
    {
        var nights = checkOutDate.DayNumber - checkInDate.DayNumber;
        return nights >= 1 && nights <= Booking.MaxNights;
    }

    /// <summary>
    ///     Checks that the room, room type and rate plan belong to the property, fit together and the guests, and that
    ///     the room is available for every night of the stay.
    /// </summary>
    /// <remarks>
    ///     An inactive room type or rate plan stays valid for a booking that already uses it.
    /// </remarks>
    private async Task<BookingError?> ValidateBookabilityAsync(int propertyId, int roomTypeId, int roomId,
        int ratePlanId, int guests, DateOnly checkInDate, DateOnly checkOutDate, Booking? currentBooking,
        CancellationToken cancellationToken)
    {
        var room = await externalRoomService.FetchRoomAsync(roomId, cancellationToken);
        if (room is null || room.PropertyId != propertyId)
            return BookingError.RoomNotFound;

        var roomType = await externalRoomService.FetchRoomTypeAsync(roomTypeId, cancellationToken);
        if (roomType is null || roomType.PropertyId != propertyId)
            return BookingError.RoomTypeNotFound;
        if (!roomType.IsActive && roomType.Id != currentBooking?.RoomTypeId)
            return BookingError.InactiveRoomType;
        if (room.RoomTypeId != roomType.Id)
            return BookingError.RoomNotInRoomType;

        var ratePlan = await externalRoomService.FetchRatePlanAsync(ratePlanId, cancellationToken);
        if (ratePlan is null || ratePlan.PropertyId != propertyId)
            return BookingError.RatePlanNotFound;
        if (!ratePlan.IsActive && ratePlan.Id != currentBooking?.RatePlanId)
            return BookingError.InactiveRatePlan;
        if (!ratePlan.Sells(roomType.Id))
            return BookingError.RatePlanNotForRoomType;

        if (guests > roomType.Capacity)
            return BookingError.OverCapacity;

        if (await bookingRepository.ExistsRoomHoldAsync(roomId, checkInDate, checkOutDate, currentBooking?.Id,
                cancellationToken)
            || await externalRoomService.IsRoomOutOfOrderAsync(roomId, checkInDate, checkOutDate, cancellationToken))
            return BookingError.RoomUnavailable;

        return null;
    }

    private static Result<Booking, BookingError> Failure(BookingError error) =>
        new Result<Booking, BookingError>.Failure(error);
}
