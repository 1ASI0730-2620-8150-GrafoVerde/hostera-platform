namespace HosteraBackend.API.Overview.Interfaces.Rest.Resources;

/// <summary>
///     Property overview resource for REST API
/// </summary>
/// <param name="PropertyId">The identifier of the property</param>
/// <param name="Name">The name of the property</param>
/// <param name="RoomsCount">The rooms of the property</param>
/// <param name="HeldRooms">The rooms held by a booking tonight</param>
/// <param name="RoomsNeedingCleaning">The rooms that need cleaning today</param>
/// <param name="RoomsOutOfOrder">The rooms blocked or out of service today</param>
/// <param name="AvailableRooms">The rooms that can still be sold tonight</param>
/// <param name="AlertsCount">The rooms that need attention today</param>
/// <param name="OccupancyRate">The share of the rooms held tonight, between 0 and 1</param>
public record PropertyOverviewResource(
    int PropertyId,
    string Name,
    int RoomsCount,
    int HeldRooms,
    int RoomsNeedingCleaning,
    int RoomsOutOfOrder,
    int AvailableRooms,
    int AlertsCount,
    decimal OccupancyRate);
