using HosteraBackend.API.AccessControl.Domain.Model.Aggregates;
using HosteraBackend.API.AccessControl.Domain.Model.Queries;

namespace HosteraBackend.API.AccessControl.Application.QueryServices;

/// <summary>
///     Represents the credential query service of the Access Control bounded context.
/// </summary>
public interface ICredentialQueryService
{
    /// <summary>
    ///     Handles the get all credentials by property id query.
    /// </summary>
    /// <param name="query">The <see cref="GetAllCredentialsByPropertyIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The credentials of the property.</returns>
    Task<IEnumerable<Credential>> Handle(GetAllCredentialsByPropertyIdQuery query, CancellationToken cancellationToken);

    /// <summary>
    ///     Handles the get credential by id query.
    /// </summary>
    /// <param name="query">The <see cref="GetCredentialByIdQuery" /> query to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The credential, or null when it does not exist.</returns>
    Task<Credential?> Handle(GetCredentialByIdQuery query, CancellationToken cancellationToken);
}
