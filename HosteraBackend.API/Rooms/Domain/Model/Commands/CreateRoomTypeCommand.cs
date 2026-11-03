namespace HosteraBackend.API.Rooms.Domain.Model.Commands;

/// <summary>
///     Command to create a room type in a property
/// </summary>
/// <param name="PropertyId">The identifier of the property that owns the room type</param>
/// <param name="Name">The name of the room type, unique within the property</param>
/// <param name="Capacity">The number of guests a room of this type can host</param>
/// <param name="BedConfiguration">The description of the beds</param>
/// <param name="BaseNightlyRate">The price of a night when no daily rate replaces it</param>
public record CreateRoomTypeCommand(
    int PropertyId,
    string Name,
    int Capacity,
    string BedConfiguration,
    decimal BaseNightlyRate);
