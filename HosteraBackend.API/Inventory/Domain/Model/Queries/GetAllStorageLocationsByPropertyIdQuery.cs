namespace HosteraBackend.API.Inventory.Domain.Model.Queries;

/// <summary>
///     Query to get all storage locations of a property
/// </summary>
/// <param name="PropertyId">The property identifier to search</param>
public record GetAllStorageLocationsByPropertyIdQuery(int PropertyId);
