using System.ComponentModel.DataAnnotations;

namespace HosteraBackend.API.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Update storage location resource for REST API. The code cannot change.
/// </summary>
/// <param name="Name">The new name of the location</param>
/// <param name="Type">The new type of the location: warehouse, closet, front-desk or service-area</param>
/// <param name="Area">The new floor or area of the location</param>
/// <param name="ResponsibleTeam">The new team in charge of the location</param>
/// <param name="Description">The new optional description of the location</param>
public record UpdateStorageLocationResource(
    [Required] [MaxLength(80)] string Name,
    [Required] string Type,
    [Required] [MaxLength(80)] string Area,
    [Required] [MaxLength(80)] string ResponsibleTeam,
    [MaxLength(240)] string? Description);
