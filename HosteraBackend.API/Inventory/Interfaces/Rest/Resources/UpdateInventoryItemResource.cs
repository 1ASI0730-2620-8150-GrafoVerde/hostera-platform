using System.ComponentModel.DataAnnotations;

namespace HosteraBackend.API.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Update inventory item resource for REST API. The code cannot change.
/// </summary>
/// <param name="Name">The new name of the item</param>
/// <param name="Category">The new category: linen, guest-amenities, cleaning-supplies or access-supplies</param>
/// <param name="Unit">The new unit: units, liters or kilograms. It cannot change after the first adjustment</param>
/// <param name="PrimaryLocationId">The identifier of the new primary storage location</param>
/// <param name="LowStockThreshold">The new quantity at which the item needs replenishment</param>
public record UpdateInventoryItemResource(
    [Required] [MaxLength(80)] string Name,
    [Required] string Category,
    [Required] string Unit,
    [Range(1, int.MaxValue)] int PrimaryLocationId,
    [Range(0, 999999)] decimal LowStockThreshold);
