using HosteraBackend.API.Rooms.Domain.Model.Commands;
using HosteraBackend.API.Rooms.Domain.Model.ValueObjects;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming an <see cref="UpdateRoomTypeResource" /> into an
///     <see cref="UpdateRoomTypeCommand" />.
/// </summary>
public static class UpdateRoomTypeCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts an <see cref="UpdateRoomTypeResource" /> to an <see cref="UpdateRoomTypeCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the new data of the room type. Must not be null.</param>
    /// <param name="roomTypeId">The identifier of the room type to update.</param>
    /// <returns>A new <see cref="UpdateRoomTypeCommand" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the status is not valid.</exception>
    public static UpdateRoomTypeCommand ToCommandFromResource(UpdateRoomTypeResource resource, int roomTypeId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new UpdateRoomTypeCommand(
            roomTypeId,
            resource.Name,
            resource.Capacity,
            resource.BedConfiguration,
            resource.BaseNightlyRate,
            RoomsEnumNames.Parse<ERecordStatus>(resource.Status, "room type status"));
    }
}
