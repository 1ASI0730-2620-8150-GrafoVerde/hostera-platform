using HosteraBackend.API.AccessControl.Domain.Model.Aggregates;
using HosteraBackend.API.AccessControl.Domain.Model.ValueObjects;
using HosteraBackend.API.AccessControl.Domain.Repositories;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.AccessControl.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework Core implementation of <see cref="ICredentialRepository" />.
/// </summary>
/// <param name="context">
///     The <see cref="AppDbContext" /> to use.
/// </param>
public class CredentialRepository(AppDbContext context)
    : BaseRepository<Credential>(context), ICredentialRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<Credential>> FindByPropertyIdAsync(int propertyId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Credential>()
            .Where(credential => credential.PropertyId == propertyId)
            .OrderByDescending(credential => credential.Id)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Credential>> FindByPropertyIdAndCardIdAsync(int propertyId, CardId cardId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Credential>()
            .Where(credential => credential.PropertyId == propertyId && credential.CardId == cardId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Credential>> FindByStaffMemberIdAsync(int staffMemberId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Credential>()
            .Where(credential => credential.StaffMemberId == staffMemberId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Credential>> FindByBookingIdAsync(int bookingId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<Credential>()
            .Where(credential => credential.BookingId == bookingId)
            .ToListAsync(cancellationToken);
    }
}
