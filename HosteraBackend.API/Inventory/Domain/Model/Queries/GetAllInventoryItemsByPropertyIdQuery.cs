namespace HosteraBackend.API.Inventory.Domain.Model.Queries;

/// <summary>
///     Query to get all inventory items of a property
/// </summary>
/// <param name="PropertyId">The property identifier to search</param>
public record GetAllInventoryItemsByPropertyIdQuery(int PropertyId);
