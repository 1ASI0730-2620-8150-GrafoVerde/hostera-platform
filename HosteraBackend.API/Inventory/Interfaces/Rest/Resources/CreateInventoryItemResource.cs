using System.ComponentModel.DataAnnotations;

namespace HosteraBackend.API.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Create inventory item resource for REST API
/// </summary>
/// <param name="Code">The code of the item, unique within the property</param>
/// <param name="Name">The name of the item</param>
/// <param name="Category">The category: linen, guest-amenities, cleaning-supplies or access-supplies</param>
/// <param name="Unit">The unit: units, liters or kilograms</param>
/// <param name="PrimaryLocationId">The identifier of the storage location where the item is usually kept</param>
/// <param name="LowStockThreshold">The quantity at which the item needs replenishment</param>
/// <param name="OpeningQuantity">
///     The quantity already on hand in the primary location; it is recorded as an opening stock adjustment
/// </param>
/// <param name="Operator">The name of the staff member who records the opening quantity; required when it is not 0</param>
public record CreateInventoryItemResource(
    [Required] [MaxLength(20)] string Code,
    [Required] [MaxLength(80)] string Name,
    [Required] string Category,
    [Required] string Unit,
    [Range(1, int.MaxValue)] int PrimaryLocationId,
    [Range(0, 999999)] decimal LowStockThreshold,
    [Range(0, 999999)] decimal OpeningQuantity = 0,
    [MaxLength(80)] string? Operator = null);
