namespace HosteraBackend.API.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to delete a storage location that no inventory item uses
/// </summary>
/// <param name="StorageLocationId">The identifier of the location to delete</param>
public record DeleteStorageLocationCommand(int StorageLocationId);
