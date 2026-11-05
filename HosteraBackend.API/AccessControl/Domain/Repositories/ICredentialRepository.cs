using HosteraBackend.API.AccessControl.Domain.Model.Aggregates;
using HosteraBackend.API.Shared.Domain.Repositories;

namespace HosteraBackend.API.AccessControl.Domain.Repositories;

/// <summary>
///     The credential repository contract.
/// </summary>
public interface ICredentialRepository : IBaseRepository<Credential>
{
    /// <summary>
    ///     Find the credentials of a property
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     An Enumerable containing the credentials of the property, or empty otherwise.
    /// </returns>
    Task<IEnumerable<Credential>> FindByPropertyIdAsync(int propertyId, CancellationToken cancellationToken = default);
}
