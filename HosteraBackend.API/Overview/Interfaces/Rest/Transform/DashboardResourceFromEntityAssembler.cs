using HosteraBackend.API.Overview.Domain.Model.ValueObjects;
using HosteraBackend.API.Overview.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Overview.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming an <see cref="OperationalDashboard" /> into a
///     <see cref="DashboardResource" /> for REST representation.
/// </summary>
public static class DashboardResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts an <see cref="OperationalDashboard" /> to its <see cref="DashboardResource" />.
    /// </summary>
    /// <param name="entity">The <see cref="OperationalDashboard" /> to convert. Must not be null.</param>
    /// <returns>A <see cref="DashboardResource" /> representing the dashboard.</returns>
    public static DashboardResource ToResourceFromEntity(OperationalDashboard entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new DashboardResource(
            entity.PropertyId,
            entity.PropertyName,
            entity.Date,
            entity.Overview is { } overview
                ? PropertyOverviewResourceFromEntityAssembler.ToResourceFromEntity(overview)
                : null,
            entity.RoomStatuses?.Counts.ToDictionary(
                count => OverviewEnumNames.ToName(count.Key),
                count => count.Value),
            entity.TodaysArrivals?.Select(arrival => new ArrivalSummaryResource(
                arrival.BookingId,
                arrival.BookingCode,
                arrival.GuestName,
                arrival.RoomNumber,
                arrival.CheckInDate,
                arrival.CheckOutDate,
                arrival.Arrived)),
            entity.DailyPerformance?.Select(day => new DailyPerformanceResource(
                day.Date,
                day.Revenue,
                day.SoldRooms,
                day.RoomsCount,
                day.OccupancyRate)),
            entity.Inventory is { } inventory
                ? new InventorySummaryResource(
                    inventory.ItemsCount,
                    inventory.LowStockItems,
                    inventory.OutOfStockItems,
                    inventory.ItemsToReplenish)
                : null,
            entity.Access is { } access
                ? new AccessSummaryResource(access.GrantedEvents, access.DeniedEvents, access.TotalEvents)
                : null,
            entity.UnavailableSections.Select(OverviewEnumNames.ToName));
    }
}
