using HosteraBackend.API.Inventory.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to create a storage location in a property
/// </summary>
/// <param name="PropertyId">The identifier of the property that owns the location</param>
/// <param name="Code">The InventoryCode of the location, unique within the property</param>
/// <param name="Name">The name shown to the staff</param>
/// <param name="Type">The EStorageLocationType of the location</param>
/// <param name="Area">The floor or area where the location is</param>
/// <param name="ResponsibleTeam">The team in charge of the location</param>
/// <param name="Description">An optional description of the location</param>
public record CreateStorageLocationCommand(
    int PropertyId,
    InventoryCode Code,
    string Name,
    EStorageLocationType Type,
    string Area,
    string ResponsibleTeam,
    string? Description);
