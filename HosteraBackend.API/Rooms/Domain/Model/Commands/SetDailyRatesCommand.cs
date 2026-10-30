namespace HosteraBackend.API.Rooms.Domain.Model.Commands;

/// <summary>
///     Command to price the nights of a room type under a rate plan
/// </summary>
/// <param name="RatePlanId">The identifier of the rate plan</param>
/// <param name="RoomTypeId">The identifier of the room type</param>
/// <param name="StartDate">The first night of the range</param>
/// <param name="EndDate">The last night of the range</param>
/// <param name="Amount">The price of each night, or null to return the nights to the base nightly rate</param>
public record SetDailyRatesCommand(
    int RatePlanId,
    int RoomTypeId,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal? Amount);
