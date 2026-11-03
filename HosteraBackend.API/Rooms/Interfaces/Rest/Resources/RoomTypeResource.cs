namespace HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

/// <summary>
///     Room type resource for REST API
/// </summary>
/// <param name="Id">The unique identifier of the room type</param>
/// <param name="PropertyId">The identifier of the property that owns the room type</param>
/// <param name="Name">The name of the room type</param>
/// <param name="Capacity">The number of guests a room of this type can host</param>
/// <param name="BedConfiguration">The description of the beds</param>
/// <param name="BaseNightlyRate">The price of a night when no daily rate replaces it</param>
/// <param name="Status">The status of the room type: active or inactive</param>
public record RoomTypeResource(
    int Id,
    int PropertyId,
    string Name,
    int Capacity,
    string BedConfiguration,
    decimal BaseNightlyRate,
    string Status);
