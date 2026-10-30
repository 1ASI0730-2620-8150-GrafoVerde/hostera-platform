using HosteraBackend.API.Overview.Domain.Model.ValueObjects;
using HosteraBackend.API.Overview.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Overview.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="PropertyOverview" /> into a
///     <see cref="PropertyOverviewResource" /> for REST representation.
/// </summary>
public static class PropertyOverviewResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a <see cref="PropertyOverview" /> to its <see cref="PropertyOverviewResource" />.
    /// </summary>
    /// <param name="entity">The <see cref="PropertyOverview" /> to convert. Must not be null.</param>
    /// <returns>A <see cref="PropertyOverviewResource" /> representing the overview.</returns>
    public static PropertyOverviewResource ToResourceFromEntity(PropertyOverview entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new PropertyOverviewResource(
            entity.PropertyId,
            entity.Name,
            entity.RoomsCount,
            entity.HeldRooms,
            entity.RoomsNeedingCleaning,
            entity.RoomsOutOfOrder,
            entity.AvailableRooms,
            entity.AlertsCount,
            entity.OccupancyRate);
    }

    /// <summary>
    ///     Converts the overviews of every property to their <see cref="PropertyOverviewsResource" />.
    /// </summary>
    /// <param name="date">The day of the overviews.</param>
    /// <param name="overviews">The overview of each property.</param>
    /// <param name="unavailableSections">The sources that could not provide data.</param>
    /// <returns>A <see cref="PropertyOverviewsResource" /> representing the comparison.</returns>
    public static PropertyOverviewsResource ToResourceFromEntities(DateOnly date,
        IEnumerable<PropertyOverview> overviews, IEnumerable<EDashboardSection> unavailableSections)
    {
        return new PropertyOverviewsResource(
            date,
            overviews.Select(ToResourceFromEntity),
            unavailableSections.Select(OverviewEnumNames.ToName));
    }
}
