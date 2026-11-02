using HosteraBackend.API.Rooms.Domain.Model.Commands;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="SetDailyRatesResource" /> into a
///     <see cref="SetDailyRatesCommand" />.
/// </summary>
public static class SetDailyRatesCommandFromResourceAssembler
{
    /// <summary>
    ///     Converts a <see cref="SetDailyRatesResource" /> to a <see cref="SetDailyRatesCommand" />.
    /// </summary>
    /// <param name="resource">The resource with the room type, range and price. Must not be null.</param>
    /// <param name="ratePlanId">The identifier of the rate plan.</param>
    /// <returns>A new <see cref="SetDailyRatesCommand" /> instance.</returns>
    public static SetDailyRatesCommand ToCommandFromResource(SetDailyRatesResource resource, int ratePlanId)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new SetDailyRatesCommand(ratePlanId, resource.RoomTypeId, resource.StartDate, resource.EndDate,
            resource.Amount);
    }
}
