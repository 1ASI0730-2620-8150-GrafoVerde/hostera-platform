using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Rooms.Domain.Model.Queries;

namespace HosteraBackend.API.Rooms.Application.QueryServices;

/// <summary>
///     Represents the property query service of the Rooms bounded context.
/// </summary>
public interface IPropertyQueryService
{
    /// <summary>
    ///     Handles the get all properties query.
    /// </summary>
    /// <param name="query">The <see cref="GetAllPropertiesQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>All the properties.</returns>
    Task<IEnumerable<Property>> Handle(GetAllPropertiesQuery query, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the get property by id query.
    /// </summary>
    /// <param name="query">The <see cref="GetPropertyByIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The property, or null when it does not exist.</returns>
    Task<Property?> Handle(GetPropertyByIdQuery query, CancellationToken cancellationToken);
}
