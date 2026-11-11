namespace HosteraBackend.API.Inventory.Domain.Model.Queries;

/// <summary>
///     Query to get a storage location by ID
/// </summary>
/// <param name="StorageLocationId">The storage location ID to search</param>
public record GetStorageLocationByIdQuery(int StorageLocationId);
