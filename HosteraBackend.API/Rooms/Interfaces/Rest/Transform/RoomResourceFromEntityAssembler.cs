using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="Room" /> aggregate into a <see cref="RoomResource" /> for
///     REST representation.
/// </summary>
public static class RoomResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a <see cref="Room" /> aggregate to its <see cref="RoomResource" />.
    /// </summary>
    /// <param name="entity">The <see cref="Room" /> to convert. Must not be null.</param>
    /// <returns>A <see cref="RoomResource" /> representing the room.</returns>
    public static RoomResource ToResourceFromEntity(Room entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new RoomResource(entity.Id, entity.PropertyId, entity.Number, entity.RoomTypeId, entity.Floor);
    }
}
