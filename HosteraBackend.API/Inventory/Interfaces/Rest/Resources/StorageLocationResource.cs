namespace HosteraBackend.API.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Storage location resource for REST API
/// </summary>
/// <param name="Id">The unique identifier of the location</param>
/// <param name="PropertyId">The identifier of the property that owns the location</param>
/// <param name="Code">The code of the location</param>
/// <param name="Name">The name of the location</param>
/// <param name="Type">The type of the location</param>
/// <param name="Area">The floor or area of the location</param>
/// <param name="ResponsibleTeam">The team in charge of the location</param>
/// <param name="Description">The optional description of the location</param>
public record StorageLocationResource(
    int Id,
    int PropertyId,
    string Code,
    string Name,
    string Type,
    string Area,
    string ResponsibleTeam,
    string? Description);
