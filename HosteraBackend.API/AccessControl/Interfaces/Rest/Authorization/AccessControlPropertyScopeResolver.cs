using HosteraBackend.API.AccessControl.Application.QueryServices;
using HosteraBackend.API.AccessControl.Domain.Model.Queries;
using HosteraBackend.API.AccessControl.Interfaces.Rest.Transform;
using HosteraBackend.API.Shared.Infrastructure.Interfaces.Asp.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HosteraBackend.API.AccessControl.Interfaces.Rest.Authorization;

/// <summary>
///     Finds the property of the credentials and access events addressed by route parameter.
/// </summary>
/// <param name="credentialQueryService">The credential query service</param>
/// <param name="accessEventQueryService">The access event query service</param>
public class AccessControlPropertyScopeResolver(
    ICredentialQueryService credentialQueryService,
    IAccessEventQueryService accessEventQueryService)
    : IPropertyScopeResolver
{
    private const string CredentialIdKey = "credentialId";
    private const string AccessEventIdKey = "accessEventId";

    /// <inheritdoc />
    public IReadOnlyCollection<string> RouteKeys { get; } = [CredentialIdKey, AccessEventIdKey];

    /// <inheritdoc />
    public async Task<int?> FindPropertyIdAsync(string routeKey, int id, CancellationToken cancellationToken)
    {
        return routeKey == CredentialIdKey
            ? (await credentialQueryService.Handle(new GetCredentialByIdQuery(id), cancellationToken))?.PropertyId
            : (await accessEventQueryService.Handle(new GetAccessEventByIdQuery(id), cancellationToken))?.PropertyId;
    }

    /// <inheritdoc />
    public IActionResult ToNotFoundResult(string routeKey, ControllerBase controller)
    {
        return routeKey == CredentialIdKey
            ? CredentialActionResultAssembler.ToActionResultFromMissingCredential(controller)
            : AccessEventActionResultAssembler.ToActionResultFromMissingAccessEvent(controller);
    }
}
