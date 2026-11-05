using HosteraBackend.API.Inventory.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to update the information of a storage location
/// </summary>
/// <remarks>
///     The code of a storage location cannot change once it is created.
/// </remarks>
/// <param name="StorageLocationId">The identifier of the location to update</param>
/// <param name="Name">The new name of the location</param>
/// <param name="Type">The new EStorageLocationType of the location</param>
/// <param name="Area">The new floor or area of the location</param>
/// <param name="ResponsibleTeam">The new team in charge of the location</param>
/// <param name="Description">The new optional description of the location</param>
public record UpdateStorageLocationCommand(
    int StorageLocationId,
    string Name,
    EStorageLocationType Type,
    string Area,
    string ResponsibleTeam,
    string? Description);
