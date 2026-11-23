using HosteraBackend.API.Communications.Domain.Model.Aggregates;
using HosteraBackend.API.Communications.Domain.Repositories;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using HosteraBackend.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HosteraBackend.API.Communications.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Entity Framework Core implementation of <see cref="IBookingConfirmationRepository" />.
/// </summary>
/// <param name="context">
///     The <see cref="AppDbContext" /> to use.
/// </param>
public class BookingConfirmationRepository(AppDbContext context)
    : BaseRepository<BookingConfirmation>(context), IBookingConfirmationRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<BookingConfirmation>> FindByBookingIdAsync(int bookingId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Set<BookingConfirmation>()
            .Where(confirmation => confirmation.BookingId == bookingId)
            .OrderByDescending(confirmation => confirmation.Id)
            .ToListAsync(cancellationToken);
    }
}
