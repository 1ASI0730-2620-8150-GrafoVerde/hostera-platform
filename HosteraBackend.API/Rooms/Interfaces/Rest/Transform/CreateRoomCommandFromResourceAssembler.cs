using HosteraBackend.API.Rooms.Domain.Model.Commands;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="CreateRoomResource" /> into a
///     <see cref="CreateRoomCommand" />.
/// </summary>
public static class CreateRoomCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a <see cref="CreateRoomResource" /> to a <see cref="CreateRoomCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the data of the room. Must not be null.</param>
    /// <param name="propertyId">The identifier of the property that owns the room.</param>
    /// <returns>A new <see cref="CreateRoomCommand" /> instance.</returns>
    public static CreateRoomCommand ToCommandFromResource(CreateRoomResource resource, int propertyId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CreateRoomCommand(propertyId, resource.Number, resource.RoomTypeId, resource.Floor);
    }
}
