using HosteraBackend.API.Rooms.Application.Internal.OutboundServices.ACL;
using HosteraBackend.API.Rooms.Application.QueryServices;
using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Queries;
using HosteraBackend.API.Rooms.Domain.Model.ValueObjects;
using HosteraBackend.API.Rooms.Domain.Repositories;

namespace HosteraBackend.API.Rooms.Application.Internal.QueryServices;

/// <summary>
///     Represents the room query service of the Rooms bounded context.
/// </summary>
/// <param name="roomRepository">
///     The <see cref="IRoomRepository" /> to use.
/// </param>
/// <param name="statusPeriodRepository">
///     The <see cref="IStatusPeriodRepository" /> to use.
/// </param>
/// <param name="externalBookingService">
///     The <see cref="ExternalBookingService" /> that reads the room assignments.
/// </param>
public class RoomQueryService(
    IRoomRepository roomRepository,
    IStatusPeriodRepository statusPeriodRepository,
    ExternalBookingService externalBookingService)
    : IRoomQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Room>> Handle(GetAllRoomsByPropertyIdQuery query,
        CancellationToken cancellationToken)
    {
        return await roomRepository.FindByPropertyIdAsync(query.PropertyId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Room?> Handle(GetRoomByIdQuery query, CancellationToken cancellationToken)
    {
        return await roomRepository.FindByIdAsync(query.RoomId, cancellationToken);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     A room assignment decides a day first (Booked, or Occupied once the guest checked in), then the status
    ///     period that covers it, and the room is Available otherwise.
    /// </remarks>
    public async Task<IEnumerable<RoomAvailability>> Handle(GetRoomAvailabilityByPropertyIdQuery query,
        CancellationToken cancellationToken)
    {
        var lastDate = query.StartDate.AddDays(GetRoomAvailabilityByPropertyIdQuery.Days - 1);
        var rooms = await roomRepository.FindByPropertyIdAsync(query.PropertyId, cancellationToken);
        var statusPeriods = (await statusPeriodRepository.FindByPropertyIdAsync(query.PropertyId, query.StartDate,
            lastDate, cancellationToken)).ToList();
        var roomAssignments = await externalBookingService.FetchRoomAssignmentsAsync(query.PropertyId,
            query.StartDate, lastDate, null, cancellationToken);

        var availability = rooms.Select(room =>
        {
            var roomStatusPeriods = statusPeriods.Where(period => period.RoomId == room.Id).ToList();
            var assignments = roomAssignments.Where(assignment => assignment.RoomId == room.Id).ToList();
            var days = Enumerable.Range(0, GetRoomAvailabilityByPropertyIdQuery.Days)
                .Select(offset => query.StartDate.AddDays(offset))
                .Select(date => new RoomDayStatus(date,
                    assignments.FirstOrDefault(assignment => assignment.Covers(date))?.DayStatus
                    ?? ToDayStatus(roomStatusPeriods, date)))
                .ToList();
            return new RoomAvailability(room, days);
        });

        return query.Status is { } status
            ? availability.Where(room => room.Days[0].Status == status).ToList()
            : availability.ToList();
    }

    private static EDayStatus ToDayStatus(IEnumerable<StatusPeriod> statusPeriods, DateOnly date)
    {
        var statusPeriod = statusPeriods.FirstOrDefault(period => period.Covers(date));
        return statusPeriod?.Status switch
        {
            EOperationalStatus.Blocked => EDayStatus.Blocked,
            EOperationalStatus.OutOfService => EDayStatus.OutOfService,
            EOperationalStatus.NeedsCleaning => EDayStatus.NeedsCleaning,
            _ => EDayStatus.Available
        };
    }
}
