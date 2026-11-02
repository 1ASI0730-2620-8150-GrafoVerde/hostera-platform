namespace HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

/// <summary>
///     Daily rate resource for REST API
/// </summary>
/// <param name="Id">The unique identifier of the daily rate</param>
/// <param name="PropertyId">The identifier of the property</param>
/// <param name="RatePlanId">The identifier of the rate plan</param>
/// <param name="RoomTypeId">The identifier of the room type</param>
/// <param name="Date">The night the price applies to</param>
/// <param name="Amount">The price of the night, which replaces the base nightly rate of the room type</param>
public record DailyRateResource(int Id, int PropertyId, int RatePlanId, int RoomTypeId, DateOnly Date, decimal Amount);
