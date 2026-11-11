using System.ComponentModel.DataAnnotations;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

/// <summary>
///     Update rate plan resource for REST API
/// </summary>
/// <param name="Name">The name of the plan, unique within the property</param>
/// <param name="Refundable">Whether the plan can be cancelled with a refund</param>
/// <param name="CancellationPolicy">The cancellation conditions, required for a refundable plan</param>
/// <param name="IncludedServices">
///     At least one service: room-only, breakfast-included, half-board or full-board
/// </param>
/// <param name="RoomTypeIds">The room types of the property sold under the plan; empty for every room type</param>
/// <param name="Status">The status of the plan: active or inactive. Plans are made inactive instead of deleted</param>
public record UpdateRatePlanResource(
    [Required] [MaxLength(80)] string Name,
    bool Refundable,
    string? CancellationPolicy,
    [Required] [MinLength(1)] IReadOnlyList<string> IncludedServices,
    [Required] IReadOnlyList<int> RoomTypeIds,
    [Required] string Status);
