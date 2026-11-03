using HosteraBackend.API.Rooms.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Rooms.Domain.Model.Commands;

/// <summary>
///     Command to update a room type
/// </summary>
/// <param name="RoomTypeId">The identifier of the room type</param>
/// <param name="Name">The name of the room type, unique within the property</param>
/// <param name="Capacity">The number of guests a room of this type can host</param>
/// <param name="BedConfiguration">The description of the beds</param>
/// <param name="BaseNightlyRate">The price of a night when no daily rate replaces it</param>
/// <param name="Status">The ERecordStatus of the room type</param>
public record UpdateRoomTypeCommand(
    int RoomTypeId,
    string Name,
    int Capacity,
    string BedConfiguration,
    decimal BaseNightlyRate,
    ERecordStatus Status);
