namespace HosteraBackend.API.Overview.Interfaces.Rest.Resources;

/// <summary>
///     Daily performance resource for REST API
/// </summary>
/// <param name="Date">The day of the night</param>
/// <param name="Revenue">The room revenue of the night</param>
/// <param name="SoldRooms">The rooms sold for the night</param>
/// <param name="RoomsCount">The rooms of the property</param>
/// <param name="OccupancyRate">The share of the rooms sold for the night, between 0 and 1</param>
public record DailyPerformanceResource(
    DateOnly Date,
    decimal Revenue,
    int SoldRooms,
    int RoomsCount,
    decimal OccupancyRate);
