using System.ComponentModel.DataAnnotations;

namespace HosteraBackend.API.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Create storage location resource for REST API
/// </summary>
/// <param name="Code">The code of the location, unique within the property</param>
/// <param name="Name">The name of the location</param>
/// <param name="Type">The type of the location: warehouse, closet, front-desk or service-area</param>
/// <param name="Area">The floor or area of the location</param>
/// <param name="ResponsibleTeam">The team in charge of the location</param>
/// <param name="Description">An optional description of the location</param>
public record CreateStorageLocationResource(
    [Required] [MaxLength(20)] string Code,
    [Required] [MaxLength(80)] string Name,
    [Required] string Type,
    [Required] [MaxLength(80)] string Area,
    [Required] [MaxLength(80)] string ResponsibleTeam,
    [MaxLength(240)] string? Description);
