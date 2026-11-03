using System.ComponentModel.DataAnnotations;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

/// <summary>
///     Update room type resource for REST API
/// </summary>
/// <param name="Name">The name of the room type, unique within the property</param>
/// <param name="Capacity">The number of guests a room of this type can host, from 1 to 12</param>
/// <param name="BedConfiguration">The description of the beds, such as "1 king + sofa bed"</param>
/// <param name="BaseNightlyRate">The price of a night when no daily rate replaces it</param>
/// <param name="Status">The status of the room type: active or inactive</param>
public record UpdateRoomTypeResource(
    [Required] [MaxLength(80)] string Name,
    [Range(1, 12)] int Capacity,
    [Required] [MaxLength(80)] string BedConfiguration,
    [Range(typeof(decimal), "0.01", "99999999.99")] decimal BaseNightlyRate,
    [Required] string Status);
