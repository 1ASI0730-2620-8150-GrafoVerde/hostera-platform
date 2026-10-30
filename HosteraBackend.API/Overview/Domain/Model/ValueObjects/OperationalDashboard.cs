namespace HosteraBackend.API.Overview.Domain.Model.ValueObjects;

/// <summary>
///     Operational overview of a property on a day
/// </summary>
/// <remarks>
///     Each part is null when a source it depends on could not provide its data. The unavailable sources are listed
///     in <see cref="UnavailableSections" />, so the dashboard keeps the information that is available.
/// </remarks>
/// <param name="PropertyId">The identifier of the property</param>
/// <param name="PropertyName">The name of the property</param>
/// <param name="Date">The day of the dashboard</param>
/// <param name="Overview">How full the property is tonight and which rooms need attention today</param>
/// <param name="RoomStatuses">The number of rooms in each day status</param>
/// <param name="TodaysArrivals">The bookings that arrive on the day</param>
/// <param name="DailyPerformance">The revenue and occupancy of each night of the requested range</param>
/// <param name="Inventory">The stock condition of the inventory items</param>
/// <param name="Access">The RFID access attempts of the day</param>
/// <param name="UnavailableSections">The sources that could not provide their data</param>
public record OperationalDashboard(
    int PropertyId,
    string PropertyName,
    DateOnly Date,
    PropertyOverview? Overview,
    RoomStatusSummary? RoomStatuses,
    IReadOnlyList<ArrivalSummary>? TodaysArrivals,
    IReadOnlyList<DailyPerformance>? DailyPerformance,
    InventorySummary? Inventory,
    AccessSummary? Access,
    IReadOnlyList<EDashboardSection> UnavailableSections);
