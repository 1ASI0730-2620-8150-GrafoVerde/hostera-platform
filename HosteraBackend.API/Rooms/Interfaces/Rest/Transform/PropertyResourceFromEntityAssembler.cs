using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Interfaces.Rest.Resources;

namespace HosteraBackend.API.Rooms.Interfaces.Rest.Transform;

/// <summary>
///     Assembler responsible for transforming a <see cref="Property" /> aggregate into a
///     <see cref="PropertyResource" /> for REST representation.
/// </summary>
public static class PropertyResourceFromEntityAssembler
{
    /// <summary>
    ///     Converts a <see cref="Property" /> aggregate to its <see cref="PropertyResource" />.
    /// </summary>
    /// <param name="entity">The <see cref="Property" /> to convert. Must not be null.</param>
    /// <returns>A <see cref="PropertyResource" /> representing the property.</returns>
    public static PropertyResource ToResourceFromEntity(Property entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new PropertyResource(entity.Id, entity.OrganizationId, entity.Name, entity.City, entity.Currency);
    }
}
