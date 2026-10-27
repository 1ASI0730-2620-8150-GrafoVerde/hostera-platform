using HosteraBackend.API.Rooms.Application.QueryServices;
using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Queries;
using HosteraBackend.API.Rooms.Domain.Repositories;

namespace HosteraBackend.API.Rooms.Application.Internal.QueryServices;

/// <summary>
///     Represents the property query service of the Rooms bounded context.
/// </summary>
/// <param name="propertyRepository">
///     The <see cref="IPropertyRepository" /> to use.
/// </param>
public class PropertyQueryService(IPropertyRepository propertyRepository) : IPropertyQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Property>> Handle(GetAllPropertiesQuery query, CancellationToken cancellationToken)
    {
        return await propertyRepository.ListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Property?> Handle(GetPropertyByIdQuery query, CancellationToken cancellationToken)
    {
        return await propertyRepository.FindByIdAsync(query.PropertyId, cancellationToken);
    }
}
