using HosteraBackend.API.Rooms.Domain.Model.Commands;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming an <see cref="UpdateRoomResource" /> into an
///     <see cref="UpdateRoomCommand" />.
/// </summary>
public static class UpdateRoomCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts an <see cref="UpdateRoomResource" /> to an <see cref="UpdateRoomCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the new data of the room. Must not be null.</param>
    /// <param name="roomId">The identifier of the room to update.</param>
    /// <returns>A new <see cref="UpdateRoomCommand" /> instance.</returns>
    public static UpdateRoomCommand ToCommandFromResource(UpdateRoomResource resource, int roomId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new UpdateRoomCommand(roomId, resource.Number, resource.RoomTypeId, resource.Floor);
    }
}
