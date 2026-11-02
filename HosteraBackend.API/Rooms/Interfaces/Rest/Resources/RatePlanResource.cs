namespace HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

/// <summary>
///     Rate plan resource for REST API
/// </summary>
/// <param name="Id">The unique identifier of the rate plan</param>
/// <param name="PropertyId">The identifier of the property that owns the plan</param>
/// <param name="Name">The name of the plan</param>
/// <param name="Currency">The three-letter code of the currency of the property, used by every price of the plan</param>
/// <param name="Refundable">Whether the plan can be cancelled with a refund</param>
/// <param name="CancellationPolicy">The cancellation conditions of a refundable plan, or null</param>
/// <param name="IncludedServices">
///     The services included in the plan: room-only, breakfast-included, half-board or full-board
/// </param>
/// <param name="RoomTypeIds">The room types sold under the plan; empty when it sells every room type</param>
/// <param name="Status">The status of the plan: active or inactive</param>
public record RatePlanResource(
    int Id,
    int PropertyId,
    string Name,
    string Currency,
    bool Refundable,
    string? CancellationPolicy,
    IEnumerable<string> IncludedServices,
    IEnumerable<int> RoomTypeIds,
    string Status);
