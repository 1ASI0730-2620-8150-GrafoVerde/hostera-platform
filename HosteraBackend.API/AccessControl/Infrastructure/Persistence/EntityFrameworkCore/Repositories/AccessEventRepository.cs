using HosteraBackend.API.AccessControl.Domain.Model.Aggregates;
using HosteraBackend.API.AccessControl.Domain.Model.Queries;
using HosteraBackend.API.AccessControl.Domain.Repositories;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.AccessControl.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework Core implementation of <see cref="IAccessEventRepository" />.
/// </summary>
/// <param name="context">
///     The <see cref="AppDbContext" /> to use.
/// </param>
public class AccessEventRepository(AppDbContext context)
    : BaseRepository<AccessEvent>(context), IAccessEventRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<AccessEvent>> FindByPropertyIdAsync(GetAllAccessEventsByPropertyIdQuery query,
        CancellationToken cancellationToken = default)
    {
        return await Matching(query)
            .OrderByDescending(accessEvent => accessEvent.OccurredAt)
            .ThenByDescending(accessEvent => accessEvent.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CountByPropertyIdAsync(GetAllAccessEventsByPropertyIdQuery query,
        CancellationToken cancellationToken = default)
    {
        return await Matching(query).CountAsync(cancellationToken);
    }

    private IQueryable<AccessEvent> Matching(GetAllAccessEventsByPropertyIdQuery query)
    {
        var accessEvents = Context.Set<AccessEvent>()
            .Where(accessEvent => accessEvent.PropertyId == query.PropertyId);

        if (query.From is { } from)
            accessEvents = accessEvents.Where(accessEvent => accessEvent.OccurredAt >= from);
        if (query.To is { } to)
            accessEvents = accessEvents.Where(accessEvent => accessEvent.OccurredAt <= to);
        if (query.Result is { } result)
            accessEvents = accessEvents.Where(accessEvent => accessEvent.Result == result);
        if (!string.IsNullOrWhiteSpace(query.HolderName))
        {
            var holderName = query.HolderName.Trim();
            accessEvents = accessEvents.Where(accessEvent => accessEvent.HolderName.Contains(holderName));
        }
        if (query.CredentialId is { } credentialId)
            accessEvents = accessEvents.Where(accessEvent => accessEvent.CredentialId == credentialId);
        if (query.CardId is { } cardId)
            accessEvents = accessEvents.Where(accessEvent => accessEvent.CardId == cardId);
        if (query.AccessPoint is { } accessPoint)
            accessEvents = accessEvents.Where(accessEvent => accessEvent.AccessPoint == accessPoint);
        if (query.RoomId is { } roomId)
            accessEvents = accessEvents.Where(accessEvent => accessEvent.RoomId == roomId);

        return accessEvents;
    }
}
