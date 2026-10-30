using HosteraBackend.API.AccessControl.Domain.Model.Aggregates;
using HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;
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

    /// <summary>
    ///     Find the credentials of a property written to a card
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="cardId">The CardId to search.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     An Enumerable containing the credentials of that card, or empty otherwise.
    /// </returns>
    Task<IEnumerable<Credential>> FindByPropertyIdAndCardIdAsync(int propertyId, CardId cardId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Find the credentials of a staff member
    /// </summary>
    /// <param name="staffMemberId">The identifier of the staff member.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    ///     An Enumerable containing the credentials of the staff member, or empty otherwise.
    /// </returns>
    Task<IEnumerable<Credential>> FindByStaffMemberIdAsync(int staffMemberId,
        CancellationToken cancellationToken = default);
}
