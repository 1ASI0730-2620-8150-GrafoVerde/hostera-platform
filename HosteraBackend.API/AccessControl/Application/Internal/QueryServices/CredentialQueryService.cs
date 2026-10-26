using HosteraBackend.API.AccessControl.Application.QueryServices;
using HosteraBackend.API.AccessControl.Domain.Model.Aggregates;
using HosteraBackend.API.AccessControl.Domain.Model.Queries;
using HosteraBackend.API.AccessControl.Domain.Repositories;

namespace HosteraBackend.API.AccessControl.Application.Internal.QueryServices;

/// <summary>
///     Represents the credential query service of the Access Control bounded context.
/// </summary>
/// <param name="credentialRepository">
///     The <see cref="ICredentialRepository" /> to use.
/// </param>
public class CredentialQueryService(ICredentialRepository credentialRepository) : ICredentialQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<Credential>> Handle(GetAllCredentialsByPropertyIdQuery query,
        CancellationToken cancellationToken)
    {
        return await credentialRepository.FindByPropertyIdAsync(query.PropertyId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Credential?> Handle(GetCredentialByIdQuery query, CancellationToken cancellationToken)
    {
        return await credentialRepository.FindByIdAsync(query.Id, cancellationToken);
    }
}
