using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="StatusPeriod" /> aggregate into a
///     <see cref="StatusPeriodResource" /> for REST representation.
/// </summary>
public static class StatusPeriodResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a <see cref="StatusPeriod" /> aggregate to its <see cref="StatusPeriodResource" />.
    /// </summary>
    /// <param name="entity">The <see cref="StatusPeriod" /> to convert. Must not be null.</param>
    /// <returns>A <see cref="StatusPeriodResource" /> representing the status period.</returns>
    public static StatusPeriodResource ToResourceFromEntity(StatusPeriod entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new StatusPeriodResource(
            entity.Id,
            entity.PropertyId,
            entity.RoomId,
            RoomsEnumNames.ToName(entity.Status),
            entity.StartDate,
            entity.EndDate,
            entity.Reason);
    }
}
