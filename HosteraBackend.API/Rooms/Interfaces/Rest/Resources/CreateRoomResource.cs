using System.ComponentModel.DataAnnotations;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

/// <summary>
///     Create room resource for REST API
/// </summary>
/// <param name="Number">The number of the room, unique within the property</param>
/// <param name="RoomTypeId">The identifier of an active room type of the same property</param>
/// <param name="Floor">The floor of the room, or null when it is not recorded</param>
public record CreateRoomResource(
    [Required] [MaxLength(10)] string Number,
    [Range(1, int.MaxValue)] int RoomTypeId,
    int? Floor);
