namespace HosteraBackend.API.Overview.Interfaces.Rest.Resources;

/// <summary>
///     Operational dashboard resource for REST API
/// </summary>
/// <param name="PropertyId">The identifier of the property</param>
/// <param name="PropertyName">The name of the property</param>
/// <param name="Date">The day of the dashboard</param>
/// <param name="Overview">How full the property is tonight, or null when its sources are unavailable</param>
/// <param name="RoomStatuses">The number of rooms in each day status, such as "needs-cleaning", or null when unavailable</param>
/// <param name="TodaysArrivals">The bookings that arrive on the day, or null when unavailable</param>
/// <param name="DailyPerformance">The revenue and occupancy of each night of the range, or null when unavailable</param>
/// <param name="Inventory">The stock condition of the inventory items, or null when unavailable</param>
/// <param name="Access">The access attempts of the day, or null when unavailable</param>
/// <param name="UnavailableSections">The sources that could not provide data: rooms, bookings, inventory or access-control</param>
public record DashboardResource(
    int PropertyId,
    string PropertyName,
    DateOnly Date,
    PropertyOverviewResource? Overview,
    IReadOnlyDictionary<string, int>? RoomStatuses,
    IEnumerable<ArrivalSummaryResource>? TodaysArrivals,
    IEnumerable<DailyPerformanceResource>? DailyPerformance,
    InventorySummaryResource? Inventory,
    AccessSummaryResource? Access,
    IEnumerable<string> UnavailableSections);
