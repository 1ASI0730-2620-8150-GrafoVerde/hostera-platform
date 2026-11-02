using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="RatePlan" /> aggregate into a
///     <see cref="RatePlanResource" /> for REST representation.
/// </summary>
public static class RatePlanResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a <see cref="RatePlan" /> aggregate to its <see cref="RatePlanResource" />.
    /// </summary>
    /// <param name="entity">The <see cref="RatePlan" /> to convert. Must not be null.</param>
    /// <param name="currency">The currency of the property that owns the plan.</param>
    /// <returns>A <see cref="RatePlanResource" /> representing the rate plan.</returns>
    public static RatePlanResource ToResourceFromEntity(RatePlan entity, string currency)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new RatePlanResource(
            entity.Id,
            entity.PropertyId,
            entity.Name,
            currency,
            entity.Refundable,
            entity.CancellationPolicy,
            entity.IncludedServices.Select(RoomsEnumNames.ToName),
            entity.RoomTypeIds.Order(),
            RoomsEnumNames.ToName(entity.Status));
    }
}
