using System.ComponentModel.DataAnnotations;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

/// <summary>
///     Set daily rates resource for REST API
/// </summary>
/// <param name="RoomTypeId">The identifier of a room type sold under the plan</param>
/// <param name="StartDate">The first night of the range</param>
/// <param name="EndDate">The last night of the range</param>
/// <param name="Amount">The price of each night, or null to return the nights to the base nightly rate</param>
public record SetDailyRatesResource(
    [Range(1, int.MaxValue)] int RoomTypeId,
    DateOnly StartDate,
    DateOnly EndDate,
    [Range(typeof(decimal), "0.01", "99999999.99")] decimal? Amount);
