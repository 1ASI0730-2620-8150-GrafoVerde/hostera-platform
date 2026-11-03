using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="DailyRate" /> aggregate into a
///     <see cref="DailyRateResource" /> for REST representation.
/// </summary>
public static class DailyRateResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a <see cref="DailyRate" /> aggregate to its <see cref="DailyRateResource" />.
    /// </summary>
    /// <param name="entity">The <see cref="DailyRate" /> to convert. Must not be null.</param>
    /// <returns>A <see cref="DailyRateResource" /> representing the daily rate.</returns>
    public static DailyRateResource ToResourceFromEntity(DailyRate entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new DailyRateResource(entity.Id, entity.PropertyId, entity.RatePlanId, entity.RoomTypeId, entity.Date,
            entity.Amount);
    }
}
