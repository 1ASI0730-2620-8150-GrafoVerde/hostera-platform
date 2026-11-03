namespace HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

/// <summary>
///     Status period resource for REST API
/// </summary>
/// <param name="Id">The unique identifier of the status period</param>
/// <param name="PropertyId">The identifier of the property that owns the room</param>
/// <param name="RoomId">The identifier of the room</param>
/// <param name="Status">The operational status: blocked, out-of-service or needs-cleaning</param>
/// <param name="StartDate">The first day of the period</param>
/// <param name="EndDate">The last day of the period</param>
/// <param name="Reason">Why the status was set</param>
public record StatusPeriodResource(
    int Id,
    int PropertyId,
    int RoomId,
    string Status,
    DateOnly StartDate,
    DateOnly EndDate,
    string Reason);
