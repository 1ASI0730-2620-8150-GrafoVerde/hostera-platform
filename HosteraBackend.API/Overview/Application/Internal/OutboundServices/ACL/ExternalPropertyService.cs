using HosteraBackend.API.Overview.Domain.Model.ReadModels;
using HosteraBackend.API.Rooms.Application.QueryServices;
using HosteraBackend.API.Rooms.Domain.Model.Queries;

namespace HosteraBackend.API.Overview.Application.Internal.OutboundServices.ACL;

/// <summary>
///     Anti-corruption layer that reads the properties of the Rooms bounded context.
/// </summary>
/// <param name="propertyQueryService">
///     The <see cref="IPropertyQueryService" /> of the Rooms bounded context.
/// </param>
public class ExternalPropertyService(IPropertyQueryService propertyQueryService)
{
    /// <summary>
    ///     Fetches a property by its identifier.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The property, or null when it does not exist.</returns>
    public async Task<PropertyReadModel?> FetchPropertyByIdAsync(int propertyId, CancellationToken cancellationToken)
    {
        var property = await propertyQueryService.Handle(new GetPropertyByIdQuery(propertyId), cancellationToken);
        return property is null ? null : new PropertyReadModel(property.Id, property.Name);
    }

    /// <summary>
    ///     Fetches every property.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The properties, ordered by name.</returns>
    public async Task<IReadOnlyList<PropertyReadModel>> FetchAllPropertiesAsync(CancellationToken cancellationToken)
    {
        var properties = await propertyQueryService.Handle(new GetAllPropertiesQuery(), cancellationToken);
        return properties
            .Select(property => new PropertyReadModel(property.Id, property.Name))
            .OrderBy(property => property.Name)
            .ToList();
    }
}
