using HosteraBackend.API.Rooms.Domain.Model.Commands;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="CreateRoomTypeResource" /> into a
///     <see cref="CreateRoomTypeCommand" />.
/// </summary>
public static class CreateRoomTypeCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a <see cref="CreateRoomTypeResource" /> to a <see cref="CreateRoomTypeCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the data of the room type. Must not be null.</param>
    /// <param name="propertyId">The identifier of the property that owns the room type.</param>
    /// <returns>A new <see cref="CreateRoomTypeCommand" /> instance.</returns>
    public static CreateRoomTypeCommand ToCommandFromResource(CreateRoomTypeResource resource, int propertyId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CreateRoomTypeCommand(
            propertyId,
            resource.Name,
            resource.Capacity,
            resource.BedConfiguration,
            resource.BaseNightlyRate);
    }
}
