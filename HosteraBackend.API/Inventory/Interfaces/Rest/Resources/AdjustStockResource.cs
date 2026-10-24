using System.ComponentModel.DataAnnotations;

namespace HosteraBackend.API.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Adjust stock resource for REST API
/// </summary>
/// <param name="Operation">The operation: in, out or transfer</param>
/// <param name="Quantity">The quantity moved, greater than zero</param>
/// <param name="StorageLocationId">The location where stock enters or leaves; the source location for a transfer</param>
/// <param name="DestinationStorageLocationId">The destination location, required only for a transfer</param>
/// <param name="Reason">The reason of the movement, such as supplier-delivery or internal-transfer</param>
/// <param name="Note">A note about the movement, required when the reason is other</param>
/// <param name="Operator">The name of the staff member who records the movement</param>
public record AdjustStockResource(
    [Required] string Operation,
    [Range(0.001, 999999)] decimal Quantity,
    [Range(1, int.MaxValue)] int StorageLocationId,
    int? DestinationStorageLocationId,
    [Required] string Reason,
    [MaxLength(240)] string? Note,
    [Required] [MaxLength(80)] string Operator);
