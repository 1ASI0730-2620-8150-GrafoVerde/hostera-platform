using System.Data.Common;
using HosteraBackend.API.AccessControl.Application.QueryServices;
using HosteraBackend.API.AccessControl.Domain.Model.Queries;
using HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;
using HosteraBackend.API.Overview.Domain.Model.ValueObjects;

namespace HosteraBackend.API.Overview.Application.Internal.OutboundServices.ACL;

/// <summary>
///     Anti-corruption layer that reads the RFID access events of the Access Control bounded context.
/// </summary>
/// <param name="accessEventQueryService">
///     The <see cref="IAccessEventQueryService" /> of the Access Control bounded context.
/// </param>
/// <param name="logger">
///     The logger used to report a source that cannot provide its data.
/// </param>
public class ExternalAccessControlService(
    IAccessEventQueryService accessEventQueryService,
    ILogger<ExternalAccessControlService> logger)
{
    /// <summary>
    ///     Fetches the number of granted and denied access attempts of a property on a day.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <param name="date">The day of the access attempts, in UTC.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The access summary, or null when the access events cannot be read.</returns>
    public async Task<AccessSummary?> FetchAccessSummaryAsync(int propertyId, DateOnly date,
        CancellationToken cancellationToken)
    {
        try
        {
            var granted = await CountAsync(propertyId, date, EAccessResult.Granted, cancellationToken);
            var denied = await CountAsync(propertyId, date, EAccessResult.Denied, cancellationToken);
            return new AccessSummary(granted, denied);
        }
        catch (DbException exception)
        {
            logger.LogWarning(exception, "The access events of property {PropertyId} could not be read.", propertyId);
            return null;
        }
    }

    private async Task<int> CountAsync(int propertyId, DateOnly date, EAccessResult result,
        CancellationToken cancellationToken)
    {
        var from = new DateTimeOffset(date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var to = new DateTimeOffset(date.ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero);
        // Only the total is needed, so the smallest page is requested
        var query = new GetAllAccessEventsByPropertyIdQuery(propertyId, from, to, result, null, null, null, null,
            null, 1, 1);
        var page = await accessEventQueryService.Handle(query, cancellationToken);
        return page.TotalCount;
    }
}
