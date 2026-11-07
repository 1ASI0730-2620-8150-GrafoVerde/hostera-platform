using HosteraBackend.API.Rooms.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Rooms.Domain.Model.Commands;

/// <summary>
///     Command to update a rate plan
/// </summary>
/// <param name="RatePlanId">The identifier of the rate plan</param>
/// <param name="Name">The name of the plan, unique within the property</param>
/// <param name="Refundable">Whether the plan can be cancelled with a refund</param>
/// <param name="CancellationPolicy">The cancellation conditions, required for a refundable plan</param>
/// <param name="IncludedServices">The services included in the plan</param>
/// <param name="RoomTypeIds">The room types sold under the plan; empty for all room types</param>
/// <param name="Status">The ERecordStatus of the plan</param>
public record UpdateRatePlanCommand(
    int RatePlanId,
    string Name,
    bool Refundable,
    string? CancellationPolicy,
    IReadOnlyList<EIncludedService> IncludedServices,
    IReadOnlyList<int> RoomTypeIds,
    ERecordStatus Status);
