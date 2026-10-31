using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="RoomType" /> aggregate into a
///     <see cref="RoomTypeResource" /> for REST representation.
/// </summary>
public static class RoomTypeResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a <see cref="RoomType" /> aggregate to its <see cref="RoomTypeResource" />.
    /// </summary>
    /// <param name="entity">The <see cref="RoomType" /> to convert. Must not be null.</param>
    /// <returns>A <see cref="RoomTypeResource" /> representing the room type.</returns>
    public static RoomTypeResource ToResourceFromEntity(RoomType entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new RoomTypeResource(
            entity.Id,
            entity.PropertyId,
            entity.Name,
            entity.Capacity,
            entity.BedConfiguration,
            entity.BaseNightlyRate,
            RoomsEnumNames.ToName(entity.Status));
    }
}
