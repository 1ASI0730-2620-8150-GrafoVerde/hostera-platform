using HosteraBackend.API.Rooms.Application.CommandServices;
using HosteraBackend.API.Rooms.Application.QueryServices;
using HosteraBackend.API.Rooms.Domain.Model.Commands;
using HosteraBackend.API.Rooms.Domain.Model.Queries;
using HosteraBackend.API.Rooms.Domain.Model.ValueObjects;
using HosteraBackend.API.Rooms.Interfaces.ACL;

namespace HosteraBackend.API.Rooms.Application.ACL;

/// <summary>
///     Facade that other bounded contexts use to read the Rooms bounded context.
/// </summary>
/// <param name="roomQueryService">The room query service</param>
/// <param name="roomTypeQueryService">The room type query service</param>
/// <param name="ratePlanQueryService">The rate plan query service</param>
/// <param name="statusPeriodQueryService">The status period query service</param>
/// <param name="statusPeriodCommandService">The status period command service</param>
public class RoomsContextFacade(
    IRoomQueryService roomQueryService,
    IRoomTypeQueryService roomTypeQueryService,
    IRatePlanQueryService ratePlanQueryService,
    IStatusPeriodQueryService statusPeriodQueryService,
    IStatusPeriodCommandService statusPeriodCommandService)
    : IRoomsContextFacade
{
    /// <inheritdoc />
    public async Task<RoomSummary?> FetchRoomAsync(int roomId, CancellationToken cancellationToken)
    {
        var room = await roomQueryService.Handle(new GetRoomByIdQuery(roomId), cancellationToken);
        return room is null ? null : new RoomSummary(room.Id, room.PropertyId, room.Number, room.RoomTypeId);
    }

    /// <inheritdoc />
    public async Task<RoomTypeSummary?> FetchRoomTypeAsync(int roomTypeId, CancellationToken cancellationToken)
    {
        var roomType = await roomTypeQueryService.Handle(new GetRoomTypeByIdQuery(roomTypeId), cancellationToken);
        return roomType is null
            ? null
            : new RoomTypeSummary(roomType.Id, roomType.PropertyId, roomType.Capacity, roomType.IsActive);
    }

    /// <inheritdoc />
    public async Task<RatePlanSummary?> FetchRatePlanAsync(int ratePlanId, CancellationToken cancellationToken)
    {
        var ratePlan = await ratePlanQueryService.Handle(new GetRatePlanByIdQuery(ratePlanId), cancellationToken);
        return ratePlan is null
            ? null
            : new RatePlanSummary(ratePlan.Id, ratePlan.PropertyId, ratePlan.Status == ERecordStatus.Active,
                ratePlan.RoomTypeIds.Count == 0, ratePlan.RoomTypeIds);
    }

    /// <inheritdoc />
    public async Task<decimal?> QuoteStayAsync(int roomTypeId, int ratePlanId, DateOnly checkInDate,
        DateOnly checkOutDate, CancellationToken cancellationToken)
    {
        var roomType = await roomTypeQueryService.Handle(new GetRoomTypeByIdQuery(roomTypeId), cancellationToken);
        if (roomType is null || checkOutDate <= checkInDate)
            return null;

        var lastNight = checkOutDate.AddDays(-1);
        var dailyRates = (await ratePlanQueryService.Handle(
                new GetDailyRatesByPropertyIdQuery(roomType.PropertyId, checkInDate, lastNight), cancellationToken))
            .Where(rate => rate.RatePlanId == ratePlanId && rate.RoomTypeId == roomTypeId)
            .ToDictionary(rate => rate.Date, rate => rate.Amount);

        var total = 0m;
        for (var night = checkInDate; night < checkOutDate; night = night.AddDays(1))
            total += dailyRates.GetValueOrDefault(night, roomType.BaseNightlyRate);

        return Math.Round(total, 2);
    }

    /// <inheritdoc />
    public async Task<bool> HasBookingBlockingStatusAsync(int roomId, DateOnly firstNight, DateOnly lastNight,
        CancellationToken cancellationToken)
    {
        var room = await roomQueryService.Handle(new GetRoomByIdQuery(roomId), cancellationToken);
        if (room is null)
            return false;

        var statusPeriods = await statusPeriodQueryService.Handle(
            new GetAllStatusPeriodsByPropertyIdQuery(room.PropertyId, firstNight, lastNight), cancellationToken);
        return statusPeriods.Any(period => period.RoomId == roomId && period.PreventsBooking);
    }

    /// <inheritdoc />
    public async Task<bool> MarkRoomForCleaningAsync(int roomId, DateOnly date, string reason,
        CancellationToken cancellationToken)
    {
        var room = await roomQueryService.Handle(new GetRoomByIdQuery(roomId), cancellationToken);
        if (room is null)
            return false;

        var statusPeriods = await statusPeriodQueryService.Handle(
            new GetAllStatusPeriodsByPropertyIdQuery(room.PropertyId, date, date), cancellationToken);
        if (statusPeriods.Any(period => period.RoomId == roomId))
            return false;

        var result = await statusPeriodCommandService.Handle(
            new SetRoomStatusCommand(roomId, EOperationalStatus.NeedsCleaning, date, date, reason), cancellationToken);
        return result.IsSuccess;
    }
}
