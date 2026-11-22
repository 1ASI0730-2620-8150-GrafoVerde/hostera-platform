using HosteraBackend.API.Rooms.Interfaces.ACL;

namespace HosteraBackend.API.Iam.Application.Internal.OutboundServices.ACL;

/// <summary>
///     Anti-corruption service that reads the organizations that own properties from the Rooms bounded context.
/// </summary>
/// <param name="roomsContextFacade">The facade of the Rooms bounded context.</param>
public class ExternalPropertyService(IRoomsContextFacade roomsContextFacade)
{
    /// <summary>
    ///     Fetches the organization that owns a property.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The identifier of the organization, or null when the property does not exist.</returns>
    public async Task<int?> FetchOrganizationIdByPropertyIdAsync(int propertyId, CancellationToken cancellationToken)
    {
        return await roomsContextFacade.FetchOrganizationIdByPropertyIdAsync(propertyId, cancellationToken);
    }

    /// <summary>
    ///     Fetches the highest organization identifier that owns a property.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The highest organization identifier, or zero when there are no properties.</returns>
    public async Task<int> FetchLatestOrganizationIdAsync(CancellationToken cancellationToken)
    {
        return await roomsContextFacade.FetchLatestOrganizationIdAsync(cancellationToken);
    }
}
