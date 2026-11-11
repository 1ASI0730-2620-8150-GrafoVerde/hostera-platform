using HosteraBackend.API.Rooms.Application.CommandServices;
using HosteraBackend.API.Rooms.Application.Errors;
using HosteraBackend.API.Rooms.Application.Internal.OutboundServices.ACL;
using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Commands;
using HosteraBackend.API.Rooms.Domain.Repositories;
using HosteraBackend.API.Shared.Application.Patterns;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.Rooms.Application.Internal.CommandServices;

/// <summary>
///     Represents the status period command service of the Rooms bounded context.
/// </summary>
/// <remarks>
///     Setting or releasing a status changes every affected period in one transaction, so a room never keeps
///     overlapping periods or a partial change.
/// </remarks>
/// <param name="statusPeriodRepository">
///     The <see cref="IStatusPeriodRepository" /> to use.
/// </param>
/// <param name="roomRepository">
///     The <see cref="IRoomRepository" /> to use.
/// </param>
/// <param name="externalBookingService">
///     The <see cref="ExternalBookingService" /> that reads which nights the bookings hold.
/// </param>
/// <param name="unitOfWork">
///     The <see cref="IUnitOfWork" /> to use.
/// </param>
public class StatusPeriodCommandService(
    IStatusPeriodRepository statusPeriodRepository,
    IRoomRepository roomRepository,
    ExternalBookingService externalBookingService,
    IUnitOfWork unitOfWork)
    : IStatusPeriodCommandService
{
    private const int MaxDaysPerRange = 366;

    /// <inheritdoc />
    public async Task<Result<IEnumerable<StatusPeriod>, RoomError>> Handle(SetRoomStatusCommand command,
        CancellationToken cancellationToken)
    {
        var days = command.EndDate.DayNumber - command.StartDate.DayNumber + 1;
        if (days < 1 || days > MaxDaysPerRange)
            return Failure(RoomError.InvalidDateRange);

        var room = await roomRepository.FindByIdAsync(command.RoomId, cancellationToken);
        if (room is null)
            return Failure(RoomError.RoomNotFound);

        StatusPeriod? newStatusPeriod = null;
        if (command.Status is { } status)
        {
            try
            {
                newStatusPeriod = new StatusPeriod(room.PropertyId, room.Id, status, command.StartDate,
                    command.EndDate, command.Reason ?? string.Empty);
            }
            catch (ArgumentException)
            {
                return Failure(RoomError.InvalidInput);
            }

            // A day controlled by a booking takes its status from the booking, never from a status period
            var roomAssignments = await externalBookingService.FetchRoomAssignmentsAsync(room.PropertyId,
                command.StartDate, command.EndDate, room.Id, cancellationToken);
            if (roomAssignments.Any(assignment => assignment.Overlaps(command.StartDate, command.EndDate)))
                return Failure(RoomError.RoomHeldByBooking);
        }

        var overlapping = await statusPeriodRepository.FindByRoomIdAndPeriodAsync(room.Id, command.StartDate,
            command.EndDate, cancellationToken);
        foreach (var statusPeriod in overlapping)
        {
            if (statusPeriod.IsWithin(command.StartDate, command.EndDate))
            {
                statusPeriodRepository.Remove(statusPeriod);
                continue;
            }

            var remainder = statusPeriod.ReleaseDays(command.StartDate, command.EndDate);
            if (remainder is not null)
                await statusPeriodRepository.AddAsync(remainder, cancellationToken);
        }

        if (newStatusPeriod is not null)
            await statusPeriodRepository.AddAsync(newStatusPeriod, cancellationToken);

        await unitOfWork.CompleteAsync(cancellationToken);

        var statusPeriods = await statusPeriodRepository.FindByRoomIdAsync(room.Id, cancellationToken);
        return new Result<IEnumerable<StatusPeriod>, RoomError>.Success(statusPeriods);
    }

    private static Result<IEnumerable<StatusPeriod>, RoomError> Failure(RoomError error) =>
        new Result<IEnumerable<StatusPeriod>, RoomError>.Failure(error);
}
