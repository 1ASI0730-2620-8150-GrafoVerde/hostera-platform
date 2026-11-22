using HosteraBackend.API.Rooms.Domain.Model.Aggregates;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.Rooms.Domain.Repositories;

/// <summary>
///     The property repository contract.
/// </summary>
public interface IPropertyRepository : IBaseRepository<Property>
{
    /// <summary>
    ///     Find the properties of an organization
    /// </summary>
    /// <param name="organizationId">The identifier of the organization.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     An Enumerable containing the properties of the organization ordered by name, or empty otherwise.
    /// </returns>
    Task<IEnumerable<Property>> FindByOrganizationIdAsync(int organizationId,
        CancellationToken cancellationToken = default);
}
